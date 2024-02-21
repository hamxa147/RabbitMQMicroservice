using Dapper;
using MassTransit;

using SharedMessages.Commands;
using SharedMessages.Response;
using ProductsMicroservice.Data;
using ProductsMicroservice.Entities;
using ProductsMicroservice.Interfaces;
using System;

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
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Success",
                        Result = new { Products = result }
                    });
                    return;
                }

                using var connection = _context.CreateConnection();
                var multiQuery = await connection.QueryMultipleAsync(query);
                var products = multiQuery.Read<Products>().ToList();

                if (products.Count > 0)
                {

                    await _redisCache.SetAsync(key, products, TimeSpan.FromMinutes(5));
                }
                await context.RespondAsync<ApiResponse>(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success",
                    Result = new { Products = products }
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("GetProductListConsumer gave exception: {Exception}", ex);
                _logger.LogInformation("GetProductListConsumer Data: {@Message}", context.Message);
                await context.RespondAsync<ApiResponse>(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Some exception occured: " + ex,
                    Result = new { }
                });
            }
        }
    }
}
