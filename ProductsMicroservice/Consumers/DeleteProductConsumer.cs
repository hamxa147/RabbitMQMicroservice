using Dapper;
using MassTransit;
using ProductsMicroservice.Data;
using ProductsMicroservice.Entities;
using ProductsMicroservice.Interfaces;
using SharedMessages.Commands;
using SharedMessages.Response;

namespace ProductsMicroservice.Consumers
{
    public class DeleteProductConsumer : IConsumer<IDeleteProduct>
    {
        private readonly DapperContext _context;
        private readonly IRedisCache _redisCache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeleteProductConsumer> _logger;

        public DeleteProductConsumer(DapperContext context, ILogger<DeleteProductConsumer> logger, IRedisCache redisCache, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _redisCache = redisCache;
            _configuration = configuration;
        }

        public async Task Consume(ConsumeContext<IDeleteProduct> context)
        {
            try
            {
                using var connection = _context.CreateConnection();
                string sql = @"DELETE Products
                             WHERE Id =@Id";

                var Id = context.Message.Id;

                var product = new { Id };
                int rowsAffected = await connection.ExecuteAsync(sql, product);

                if (rowsAffected > 0)
                {
                    var key = _configuration["ProductCacheKey"] + Id;
                    var listKey = _configuration["ProductCacheKey"] + "list";

                    await _redisCache.RemoveAsync(key);
                    var productList = await _redisCache.GetAsync<List<Products>>(listKey);

                    var productToRemove = productList.FirstOrDefault(x => x.Id == Id);

                    if (productToRemove != null)
                    {
                        productList.Remove(productToRemove);
                    }

                    await _redisCache.UpdateAsync(listKey, productList, TimeSpan.FromMinutes(5));

                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Success",
                        Result = new { Product = productToRemove }
                    });
                }
                else
                {
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Product not found: ",
                        Result = new { }
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("DeleteProductConsumer gave exception: {Exception}", ex);
                _logger.LogInformation("DeleteProductConsumer Data: {@Message}", context.Message);
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
