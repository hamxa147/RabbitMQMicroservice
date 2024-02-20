using AuthenticationMicroservice.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddMassTransit(busConfigurator =>
{
    // Register consumers
    busConfigurator.AddConsumer<AuthenticationConsumer>();

    // Configure RabbitMQ
    busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        busFactoryConfigurator.Host(configuration["Rabbitmq:Url"], h =>
        {
            h.Username(configuration["Rabbitmq:Username"]);
            h.Password(configuration["Rabbitmq:Password"]);
        });

        busFactoryConfigurator.ReceiveEndpoint("authentication", ep =>
        {
            ep.ConfigureConsumer<AuthenticationConsumer>(context);
        });
    });
    //busConfigurator.AddRequestClient<ILoginEvent>();
});

var app = builder.Build();

app.Run();
