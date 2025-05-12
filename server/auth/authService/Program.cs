using authService;
using authService.Repositories;
using authService.Services;
using Common.Events;
using IdentityServer4.Validation;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddSingleton<IUserRepository>(new UserRepository(connectionString));
builder.Services.AddScoped<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
builder.Services.AddLogging(logging => logging.AddConsole());
// builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.Publish<IUserLoggedInEvent>(x =>{    x.Exclude = true;});
        cfg.Publish<IUserRegisterEvent>(x =>{    x.Exclude = true;});
        cfg.ConfigureEndpoints(context); // Auto-configures endpoints for consumers
    });
});

// Add publisher service
builder.Services.AddScoped<RabbitMQPublisher>();

builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(Config.ApiScopes)
     .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
    .AddProfileService<CustomProfileService>()
    .AddDeveloperSigningCredential();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cooleee", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.UseRouting();
    app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
