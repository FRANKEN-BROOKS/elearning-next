using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.MessageQueue;
using SubscriptionService.Infrastructure.Data;
using SubscriptionService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Subscription Service API", 
        Version = "v1",
        Description = "E-Learning Platform - Subscription & Enrollment Service"
    });
});

builder.Services.AddDbContext<SubscriptionDbContext>(options =>
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

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<SubscriptionDbContext>();

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