using BuildingBlocks.Exceptions.Handler;
using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Npgsql.Replication.PgOutput.Messages;

var builder = WebApplication.CreateBuilder(args);

// Add Services to the DI Container
var assembly = typeof(Program).Assembly;

#region Application Services
builder.Services.AddCarter();
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(assembly);
    options.AddOpenBehavior(typeof(LoggingBehavior<,>));
    options.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
#endregion

#region Data Services
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
    options.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
// Here we are using the decorator pattern to add caching functionality to the BasketRepository
// using scrutor library
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")!;
    // options.InstanceName = "BasketInstance";
});

//•	Here, a factory method is used to create an instance of CachedBasketRepository,
//  which wraps the BasketRepository and uses IDistributedCache for caching.
//builder.Services.AddScoped<IBasketRepository>(provider =>
//{
//   var basketRepository = provider.GetRequiredService<BasketRepository>();
//   return new CachedBasketRepository(basketRepository, provider.GetRequiredService<IDistributedCache>());
//});
#endregion

#region Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});
#endregion

#region Cross-cutting services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

//builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
//{
//    options.Address = new Uri(discountUrl);
//});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    });

app.Run();
