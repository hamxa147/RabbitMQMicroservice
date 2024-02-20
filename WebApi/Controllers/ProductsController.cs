using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // Connect to RabbitMQ
            // Send parameters to the queue and wait for response
            // Map the response and send it back to user
            return Ok();
        }

        [HttpPost]
        public IActionResult AddPost()
        {
            // Connect to RabbitMQ
            // Send parameters to the queue and wait for response
            // Map the response and send it back to user
            return Ok();
        }

        [HttpPost]
        [Route("update")]
        public IActionResult UpdatePost()
        {
            // Connect to RabbitMQ
            // Send parameters to the queue and wait for response
            // Map the response and send it back to user
            return Ok();
        }
    }
}
