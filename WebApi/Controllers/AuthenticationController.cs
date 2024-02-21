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
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IRequestClient<IAuthenticateUser> _loginEventRequestClient;

        public AuthenticationController(ILogger<AuthenticationController> logger, IRequestClient<IAuthenticateUser> loginEventRequestClient)
        {
            _logger = logger;
            _loginEventRequestClient = loginEventRequestClient;
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthenticationDTO authentication)
        {
            try
            {
                // Using MassTransit IRequestClient we send request and recieve resonse
                var response = await _loginEventRequestClient.GetResponse<ApiResponse>(new
                {
                    authentication.Email,
                    authentication.Password
                });

                var loginResponse = response.Message;

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Authentication Controller gave exception: {Exception}", ex);
                return BadRequest();
            }
        }
    }
}
