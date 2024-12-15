using Api.Services;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using UserService.Api.Extensions;
using UserService.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    // Puerto para HTTP con HTTP/1.1
    options.ListenAnyIP(5003, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // Puerto para gRPC con HTTP/2
    options.ListenAnyIP(5013, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// Agregar servicios para gRPC
builder.Services.AddGrpc().AddJsonTranscoding();

// Agregar servicios de aplicaciÃ³n (como tu UserAuthService, UsersService, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Cache());
});


// Configurar MassTransit a RabiitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Send<User>(config =>
        {
            config.UseRoutingKeyFormatter(contextt => "user-queue");
        });

    });
});

var app = builder.Build();
app.UseOutputCache();

// Seed de la base de datos
AppSeedService.SeedDatabase(app);

// Configurar el mapeo de servicios gRPC
app.MapGrpcService<UserAuthService>();
app.MapGrpcService<UsersService>();

app.Run();