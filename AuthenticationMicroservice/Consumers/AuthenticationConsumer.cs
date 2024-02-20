using AuthenticationMicroservice.Authentication;
using AuthenticationMicroservice.Data;
using AuthenticationMicroservice.Entity;
using AuthenticationMicroservice.Interfaces;
using Dapper;
using MassTransit;
using SharedMessages.Events;
using SharedMessages.Response;

namespace AuthenticationMicroservice.Consumers
{
    public class AuthenticationConsumer : IConsumer<ILoginEvent>
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

        public async Task Consume(ConsumeContext<ILoginEvent> context)
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
                    await context.RespondAsync<AuthenticationResponse>(new
                    {
                        Token = "",
                    });
                }
                else
                {
                    var token = _jwtTokenGenerator.GenerateToken(user);
                    await context.RespondAsync<AuthenticationResponse>(new
                    {
                        Token = "Found user",
                    });
                }
            }
            catch(Exception ex)
            {
                await context.RespondAsync<AuthenticationResponse>(new
                {
                    Token = "Exception",
                });
            }
        }
    }
}
