using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.MessageQueue;
using CertificateService.Infrastructure.Data;
using CertificateService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Certificate Service API", 
        Version = "v1",
        Description = "E-Learning Platform - Certificate Generation Service"
    });
});

builder.Services.AddDbContext<CertificateDbContext>(options =>
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

builder.Services.AddScoped<ICertificateGenerator, CertificateGenerator>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<CertificateDbContext>();

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

// Serve static files (certificates)
app.UseStaticFiles();

app.Run();