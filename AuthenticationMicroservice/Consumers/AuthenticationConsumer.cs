using Dapper;
using MassTransit;

using SharedMessages.Commands;
using SharedMessages.Response;
using AuthenticationMicroservice.Data;
using AuthenticationMicroservice.Entities;
using AuthenticationMicroservice.Interfaces;

namespace AuthenticationMicroservice.Consumers
{
    public class AuthenticationConsumer : IConsumer<IAuthenticateUser>
    {
        private readonly DapperContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthenticationConsumer> _logger;

        public AuthenticationConsumer(DapperContext context, IJwtTokenGenerator jwtTokenGenerator, ILogger<AuthenticationConsumer> logger)
        {
            _logger = logger;
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task Consume(ConsumeContext<IAuthenticateUser> context)
        {
            try
            {
                var Email = context.Message.Email;
                var Password = context.Message.Password;

                var query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";

                using var connection = _context.CreateConnection();
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { Email, Password });

                if (user == null)
                {
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Invalid Credentials",
                        Result = new { Token = "" }
                    });
                }
                else
                {
                    var token = _jwtTokenGenerator.GenerateToken(user);
                    await context.RespondAsync<ApiResponse>(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Token Generated Sucessfully",
                        Result = new { Token = token }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Authentication Consumer gave exception: {Exception}", ex);
                _logger.LogInformation("Authentication Consumer Data: {@Message}", context.Message);
                await context.RespondAsync<ApiResponse>(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Some exception occured: " + ex,
                    Result = new { Token = "" }
                });
            }
        }
    }
}
