using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.MessageQueue;
using PaymentService.Infrastructure.Data;
using PaymentService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Payment Service API", 
        Version = "v1",
        Description = "E-Learning Platform - Payment Processing Service with Omise"
    });
});

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddSingleton<IMessageQueueService>(provider =>
    new RabbitMQService(
        rabbitMQSettings["Host"],
        rabbitMQSettings["Username"],
        rabbitMQSettings["Password"]
    ));

builder.Services.AddHttpClient<IOmiseService, OmiseService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PaymentDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();