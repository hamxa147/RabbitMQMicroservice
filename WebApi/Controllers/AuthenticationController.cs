using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedMessages.Commands;
using SharedMessages.Response;
using WebApi.Models.Authentication;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IRequestClient<IAuthenticateUser> _loginEventRequestClient;
        private readonly ILogger<AuthenticationDTO> _logger;

        public AuthenticationController(ILogger<AuthenticationDTO> logger, IRequestClient<IAuthenticateUser> loginEventRequestClient)
        {
            _logger = logger;
            _loginEventRequestClient = loginEventRequestClient;
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthenticationDTO authentication)
        {
            _logger.Log(LogLevel.Information, "Schedule has been created");
            var response = await _loginEventRequestClient.GetResponse<AuthenticationResponse>(new
            {
                authentication.Email,
                authentication.Password
            });
            // Receive some data from the parameters
            // Send that data to RabbitMQ
            // Wait for the reponse
            // Check the response and based on response send back appropiate status code
            var loginResponse = response.Message;

            return Ok(loginResponse);
        }
    }
}
