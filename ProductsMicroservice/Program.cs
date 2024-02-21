using MassTransit;
using ProductsMicroservice.Consumers;
using ProductsMicroservice.Data;
using ProductsMicroservice.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddSingleton<DapperContext>();

//builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowSpecificOrigin",
//         builder =>
//         {
//             builder.WithOrigins("http://localhost:5284")
//                    .AllowAnyHeader()
//                    .AllowAnyMethod()
//                    .AllowCredentials();
//         });
// });

//builder.Services.AddSignalR();

builder.Services.AddMassTransit(busConfigurator =>
{
    // Register consumers
    busConfigurator.AddConsumer<CreateProductConsumer>();
    busConfigurator.AddConsumer<DeleteProductConsumer>();
    busConfigurator.AddConsumer<UpdateProductConsumer>();
    busConfigurator.AddConsumer<GetProductByIdConsumer>();
    busConfigurator.AddConsumer<GetProductListConsumer>();

    // Configure RabbitMQ
    busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        busFactoryConfigurator.Host(configuration["Rabbitmq:Url"], h =>
        {
            h.Username(configuration["Rabbitmq:Username"]);
            h.Password(configuration["Rabbitmq:Password"]);
        });

        busFactoryConfigurator.ReceiveEndpoint("get-product", ep =>
        {
            ep.ConfigureConsumer<GetProductListConsumer>(context);
        });
        busFactoryConfigurator.ReceiveEndpoint("get-product-by-id", ep =>
        {
            ep.ConfigureConsumer<GetProductByIdConsumer>(context);
        });
        busFactoryConfigurator.ReceiveEndpoint("create-product", ep =>
        {
            ep.ConfigureConsumer<CreateProductConsumer>(context);
        });
        busFactoryConfigurator.ReceiveEndpoint("update-product", ep =>
        {
            ep.ConfigureConsumer<UpdateProductConsumer>(context);
        });
        busFactoryConfigurator.ReceiveEndpoint("delete-product", ep =>
        {
            ep.ConfigureConsumer<DeleteProductConsumer>(context);
        });
    });

    builder.Services.AddSingleton<IRedisCache, RedisCacheService>();
    builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = configuration["RedisCacheUrl"]; });

    //busConfigurator.AddRequestClient<ILoginEvent>();
});

var app = builder.Build();

//app.UseCors("AllowSpecificOrigin");

//app.MapHub<ProductHub>("/productHub");

app.Run();
