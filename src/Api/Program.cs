
using Api.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using UserService.Api.Extensions;

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

// Agregar servicios de aplicación (como tu UserAuthService, UsersService, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Cache());
});

var app = builder.Build();
app.UseOutputCache();

// Seed de la base de datos
AppSeedService.SeedDatabase(app);

// Configurar el mapeo de servicios gRPC
app.MapGrpcService<UserAuthService>();
app.MapGrpcService<UsersService>();

app.Run();
