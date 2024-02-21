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
    public class GetProductByIdConsumer : IConsumer<IGetProduct>
    {
        private readonly DapperContext _context;
        private readonly IRedisCache _redisCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GetProductByIdConsumer> _logger;

        public GetProductByIdConsumer(DapperContext context, ILogger<GetProductByIdConsumer> logger, IRedisCache redisCache, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _redisCache = redisCache;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<IGetProduct> context)
        {
            try
            {
                var Id = context.Message.Id;

                var key = _configuration["ProductCacheKey"] + Id;

                var result = await _redisCache.GetAsync<Products>(key);

                if (result is not null)
                {
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Success",
                        Result = new { Product = result }
                    });
                    return;
                }

                var query = "SELECT * FROM Products WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                var product = await connection.QuerySingleOrDefaultAsync<Products>(query, new { Id });

                if (product is null)
                {
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Not found",
                        Result = new {  }
                    });
                }
                else
                {
                    await _redisCache.SetAsync(key, product, TimeSpan.FromMinutes(5));
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Success",
                        Result = new { Product = product }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("GetProductByIdConsumer gave exception: {Exception}", ex);
                _logger.LogInformation("GetProductByIdConsumer Data: {@Message}", context.Message);
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
