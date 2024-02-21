using Dapper;
using MassTransit;

using SharedMessages.Commands;
using SharedMessages.Response;
using ProductsMicroservice.Data;
using ProductsMicroservice.Entities;
using ProductsMicroservice.Interfaces;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace ProductsMicroservice.Consumers
{
    public class GetProductListConsumer : IConsumer<IGetProductList>
    {
        private readonly DapperContext _context;
        private readonly IRedisCache _redisCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GetProductListConsumer> _logger;

        public GetProductListConsumer(DapperContext context, ILogger<GetProductListConsumer> logger, IRedisCache redisCache, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _redisCache = redisCache;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<IGetProductList> context)
        {
            try
            {
                var key = _configuration["ProductCacheKey"] + "list";

                var query = "SELECT * FROM Products";

                var result = await _redisCache.GetAsync<List<Products>>(key);

                if (result is not null)
                {
                    await context.RespondAsync<ProductListResponse>(new
                    {
                        Products = result,
                    });
                    return;
                }

                using var connection = _context.CreateConnection();
                var multiQuery = await connection.QueryMultipleAsync(query);
                var products = multiQuery.Read<Products>().ToList();

                if (products.Count > 0)
                {
                    await _redisCache.SetAsync(key, products, TimeSpan.FromMinutes(5));
                    await context.RespondAsync<ProductListResponse>(new
                    {
                        Products = products,
                    });
                }
                else
                {
                    await context.RespondAsync<ProductListResponse>(new
                    {
                        Products = new List<Products>()
                    });
                }
            }
            catch (Exception ex)
            {
                await context.RespondAsync<ProductListResponse>(new
                {
                    Products = new List<Products>(),
                });
            }
        }
    }
}
