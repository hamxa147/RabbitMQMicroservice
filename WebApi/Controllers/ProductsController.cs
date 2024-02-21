using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using WebApi.Hubs;
using WebApi.Authorization;
using WebApi.Models.Products;
using SharedMessages.Commands;
using SharedMessages.Response;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IHubContext<ProductHub> _hubContext;
        private readonly ILogger<ProductsController> _logger;
        private readonly IRequestClient<IGetProduct> _getProductRequestClient;
        private readonly IRequestClient<ICreateProduct> _createProductRequestClient;
        private readonly IRequestClient<IUpdateProduct> _updateProductRequestClient;
        private readonly IRequestClient<IDeleteProduct> _deleteProductRequestClient;
        private readonly IRequestClient<IGetProductList> _getProductRequestListClient;

        public ProductsController(
            IHubContext<ProductHub> hubContext,
            ILogger<ProductsController> logger,
            IRequestClient<IGetProduct> getProductRequestClient,
            IRequestClient<ICreateProduct> createProductRequestClient,
            IRequestClient<IUpdateProduct> updateProductRequestClient,
            IRequestClient<IDeleteProduct> deleteProductRequestClient,
            IRequestClient<IGetProductList> getProductRequestListClient)
        {
            _logger = logger;
            _hubContext = hubContext;
            _getProductRequestClient = getProductRequestClient;
            _createProductRequestClient = createProductRequestClient;
            _updateProductRequestClient = updateProductRequestClient;
            _deleteProductRequestClient = deleteProductRequestClient;
            _getProductRequestListClient = getProductRequestListClient;
        }

        [HttpGet]
        [Authorization("HasReadAccess")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _getProductRequestListClient.GetResponse<ApiResponse>(new { });
                var result = response.Message;

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Product Controller gave exception for GetAll. Exception: {Exception}", ex);
                return BadRequest();
            }
        }

        [HttpGet("{id:int}")]
        [Authorization("HasReadAccess")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var response = await _getProductRequestClient.GetResponse<ApiResponse>(new { id });
                var result = response.Message;

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Product Controller gave exception for GetById. Exception: {Exception}", ex);
                return BadRequest();
            }
        }

        [HttpPost("{name}")]
        [Authorization("HasCreateAccess")]
        public async Task<IActionResult> AddProduct(string name)
        {
            var response = await _createProductRequestClient.GetResponse<ApiResponse>(new { name });
            var result = response.Message;

            await _hubContext.Clients.All.SendAsync("NofityLogs", result, "Create new product logs: ");

            return Ok(result);
        }

        [HttpPut]
        [Authorization("HasWriteAccess")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var response = await _updateProductRequestClient.GetResponse<ApiResponse>(new { product.Name, product.Id });
            var result = response.Message;

            await _hubContext.Clients.All.SendAsync("NofityLogs", result, "Product update logs: ");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorization("HasDeleteAccess")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _deleteProductRequestClient.GetResponse<ApiResponse>(new { id });
            var result = response.Message;
            
            await _hubContext.Clients.All.SendAsync("NofityLogs", result, "Product deletion logs: ");

            return Ok(result);
        }
    }
}
