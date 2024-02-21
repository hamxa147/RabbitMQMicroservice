﻿using Dapper;
using MassTransit;
using ProductsMicroservice.Data;
using ProductsMicroservice.Entities;
using ProductsMicroservice.Interfaces;
using SharedMessages.Commands;
using SharedMessages.Response;

namespace ProductsMicroservice.Consumers
{
    public class CreateProductConsumer : IConsumer<ICreateProduct>
    {
        private readonly DapperContext _context;
        private readonly IRedisCache _redisCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateProductConsumer> _logger;

        public CreateProductConsumer(DapperContext context, ILogger<CreateProductConsumer> logger, IRedisCache redisCache, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _redisCache = redisCache;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<ICreateProduct> context)
        {
            try
            {
                using var connection = _context.CreateConnection();
                string sql = @"INSERT INTO Products (Name)
                             OUTPUT INSERTED.[Id]
                             VALUES (@Name);";

                var productName = context.Message.Name;

                var product = new { Name = productName };
                int productId = await connection.QuerySingleAsync<int>(sql, new { context.Message.Name });

                if (productId > 0)
                {
                    var key = _configuration["ProductCacheKey"] + productId;
                    var listKey = _configuration["ProductCacheKey"] + "list";
                    
                    var newProduct = new Products() { Id = productId, Name = productName };
                    await _redisCache.SetAsync(key, newProduct, TimeSpan.FromMinutes(5));

                    // Fetch all the product list and update the instance for the updated product
                    var productList = await _redisCache.GetAsync<List<Products>>(listKey);
                    productList.Add(newProduct);

                    await _redisCache.UpdateAsync(listKey, productList, TimeSpan.FromMinutes(5));

                    await context.RespondAsync<ProductResponse>(new
                    {
                        Id = productId,
                        Name = productName
                    });
                }
                else
                {
                    await context.RespondAsync<ProductResponse>(null);
                }

            }
            catch (Exception ex)
            {
                await context.RespondAsync<ProductResponse>(null);
            }
        }
    }
}
