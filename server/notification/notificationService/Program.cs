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
    x.AddConsumer<OrderCreatedConsumer>();

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
        cfg.ReceiveEndpoint("order-created-notification-queue", e =>
        {
            e.Bind("Common.Events:OrderCreatedEvent");
             e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});





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
