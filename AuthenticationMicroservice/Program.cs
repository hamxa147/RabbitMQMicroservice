using MassTransit;
using Microsoft.Extensions.Options;
using AuthenticationMicroservice.Data;
using AuthenticationMicroservice.Consumers;
using AuthenticationMicroservice.Interfaces;
using AuthenticationMicroservice.Authentication;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddSingleton<DapperContext>();

var jwtSettings = new JwtSettings();
configuration.Bind(JwtSettings.SectionName, jwtSettings);

builder.Services.AddSingleton(Options.Create(jwtSettings));

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

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
