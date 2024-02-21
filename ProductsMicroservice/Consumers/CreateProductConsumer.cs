using Dapper;
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

                int productId = await connection.QuerySingleAsync<int>(sql, new { context.Message.Name });

                if (productId > 0)
                {
                    var key = _configuration["ProductCacheKey"] + productId;
                    var listKey = _configuration["ProductCacheKey"] + "list";

                    var newProduct = new Products() { Id = productId, Name = productName };
                    await _redisCache.SetAsync(key, newProduct, TimeSpan.FromMinutes(5));

                    // Fetch all the product list and update the instance for the updated product
                    var productList = await _redisCache.GetAsync<List<Products>>(listKey);
                    if (productList?.Count > 0)
                    {
                        productList.Add(newProduct);
                    }

                    await _redisCache.UpdateAsync(listKey, productList, TimeSpan.FromMinutes(5));

                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Success",
                        Result = new { Product = newProduct }
                    });
                }
                else
                {
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Product not found",
                        Result = new { }
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("CreateProductConsumer gave exception: {Exception}", ex);
                _logger.LogInformation("CreateProductConsumer Data: {@Message}", context.Message);
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
