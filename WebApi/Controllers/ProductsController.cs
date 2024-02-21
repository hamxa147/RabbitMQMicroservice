using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedMessages.Commands;
using SharedMessages.Response;
using System.Xml.Linq;
using WebApi.Authorization;
using WebApi.Models.Products;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(AuthorizationCheckOperation))]
    public class ProductsController : ControllerBase
    {
        private readonly IRequestClient<IGetProduct> _getProductRequestClient;
        private readonly IRequestClient<ICreateProduct> _createProductRequestClient;
        private readonly IRequestClient<IUpdateProduct> _updateProductRequestClient;
        private readonly IRequestClient<IDeleteProduct> _deleteProductRequestClient;
        private readonly IRequestClient<IGetProductList> _getProductRequestListClient;

        public ProductsController(
            IRequestClient<IGetProduct> getProductRequestClient,
            IRequestClient<ICreateProduct> createProductRequestClient,
            IRequestClient<IUpdateProduct> updateProductRequestClient,
            IRequestClient<IDeleteProduct> deleteProductRequestClient,
            IRequestClient<IGetProductList> getProductRequestListClient)
        {
            _getProductRequestClient = getProductRequestClient;
            _createProductRequestClient = createProductRequestClient;
            _updateProductRequestClient = updateProductRequestClient;
            _deleteProductRequestClient = deleteProductRequestClient;
            _getProductRequestListClient = getProductRequestListClient;
        }

        [HttpGet]
        [Authorization("HasReadAccess")]
        public async Task<IActionResult> Get()
        {
            var response = await _getProductRequestListClient.GetResponse<ProductListResponse>(new { });
            var result = response.Message;

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorization("HasReadAccess")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _getProductRequestClient.GetResponse<ProductResponse>(new { id });
            var result = response.Message;

            return Ok(result);
        }

        [HttpPost("{name}")]
        [Authorization("HasCreateAccess")]
        public async Task<IActionResult> AddProduct(string name)
        {
            var response = await _createProductRequestClient.GetResponse<ProductResponse>(new { name });
            var result = response.Message;

            return Ok(result);
        }

        [HttpPut]
        [Authorization("HasWriteAccess")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var response = await _updateProductRequestClient.GetResponse<ProductResponse>(new { product.Name, product.Id });
            var result = response.Message;

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorization("HasDeleteAccess")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _deleteProductRequestClient.GetResponse<ProductResponse>(new { id });
            var result = response.Message;

            return Ok(result);
        }
    }
}
