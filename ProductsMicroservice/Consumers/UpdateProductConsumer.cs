using Dapper;
using MassTransit;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ProductsMicroservice.Data;
using ProductsMicroservice.Entities;
using ProductsMicroservice.Interfaces;
using SharedMessages.Commands;
using SharedMessages.Response;

namespace ProductsMicroservice.Consumers
{
    public class UpdateProductConsumer : IConsumer<IUpdateProduct>
    {
        private readonly DapperContext _context;
        private readonly IRedisCache _redisCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateProductConsumer> _logger;

        public UpdateProductConsumer(DapperContext context, ILogger<CreateProductConsumer> logger, IRedisCache redisCache, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _redisCache = redisCache;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<IUpdateProduct> context)
        {
            try
            {
                var Id = context.Message.Id;
                var Name = context.Message.Name;

                var key = _configuration["ProductCacheKey"] + Id;
                var listKey = _configuration["ProductCacheKey"] + "list";

                using var connection = _context.CreateConnection();
                string sql = @"Update Products
                             SET Name =@Name
                             WHERE Id =@Id";

                var product = new Products() { Name = Name, Id = Id };
                int rowsAffected = await connection.ExecuteAsync(sql, product);

                if (rowsAffected > 0)
                {
                    // Update the existing instance and if it doesn't exist create it
                    await _redisCache.UpdateAsync(key, product, TimeSpan.FromMinutes(5));

                    // Fetch all the product list and update the instance for the updated product
                    var productList = await _redisCache.GetAsync<List<Products>>(listKey);
                    if (productList.Count > 0)
                    {
                        var updatedProduct = productList.Find((x) => x.Id == Id);
                        if (updatedProduct != null)
                        {
                            updatedProduct.Name = product.Name;

                            await _redisCache.UpdateAsync(listKey, productList, TimeSpan.FromMinutes(5));
                        }
                    }

                    await context.RespondAsync<ProductResponse>(new
                    {
                        Id,
                        Name
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
