using Api.Services;
using UserService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<UserAuthService>();
app.MapGrpcService<UsersService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Database seeding
AppSeedService.SeedDatabase(app);

app.Run();
