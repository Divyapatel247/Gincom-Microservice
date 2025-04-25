using MassTransit;
using MassTransit.Transports.Fabric;
using notificationService.Consumers;
using notificationService.Hubs;
using notificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddScoped<NotificationServiceForOrderCreated>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserLoggedInConsumer>();
    x.AddConsumer<UserRegisterConsumer>();
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<ProductStockUpdateConsumer>();
    x.AddConsumer<OrderStatusUpdatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("a", e =>
        {
            e.Bind("xxxx");
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<UserLoggedInConsumer>(context);
            // e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
        cfg.ReceiveEndpoint("publish-user-register-queue", e =>
        {
            e.Bind("PublishUserRegister");
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<UserRegisterConsumer>(context);
            // e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
        cfg.ReceiveEndpoint("order-created-notification-queue", e =>
        {
            e.Bind("Common.Events:OrderCreatedEvent");
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
        cfg.ReceiveEndpoint("product-stock-updated-queue", e =>
        {
            e.Bind("Common.Events:ProductUpdatedStock");
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<ProductStockUpdateConsumer>(context);
        });
        cfg.ReceiveEndpoint("order-status-updated-queue", e =>
        {
            e.Bind("Common.Events:OrderStatusUpdatedEvent");
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<OrderStatusUpdatedConsumer>(context);
        });
    });
});



builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapControllers();
});
app.Run();
