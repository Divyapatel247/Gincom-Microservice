using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// Add JWT authentication

builder.Services.AddAuthentication()
    .AddJwtBearer("IdentityServer", options =>
    {
        options.Authority = "http://localhost:5001";
        options.RequireHttpsMetadata = false;
        options.Audience = "api.read";
    });


builder.Services.AddAuthorization(options =>
{
options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer("Bearer", options =>
//     {
//         options.Authority = "https://localhost:5001"; // Auth Service URL
//         options.Audience = "product-service"; // Expected audience
//         options.RequireHttpsMetadata = false; // For development
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true
//         };
//     });

// Add CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // For SignalR
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
// app.UseAuthentication();
// app.UseAuthorization();
app.UseOcelot().Wait();



app.Run();

