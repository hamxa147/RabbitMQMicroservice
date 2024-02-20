using MassTransit;
using SharedMessages.Events;
using SharedMessages.Response;

namespace AuthenticationMicroservice.Consumers
{
    public class AuthenticationConsumer : IConsumer<ILoginEvent>
    {
        private readonly ILogger<AuthenticationConsumer> _logger;
        public AuthenticationConsumer(ILogger<AuthenticationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ILoginEvent> context)
        {
            await context.RespondAsync<AuthenticationResponse>(new
            {
                Token = "",
                aa = "TESTTEST",
                Email = "dalkmndwelknmflekw",
                Password = "DOGGGGG"
            });
        }
    }
}
