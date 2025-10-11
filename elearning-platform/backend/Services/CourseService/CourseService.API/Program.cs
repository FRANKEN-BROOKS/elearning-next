using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.MessageQueue;
using CourseService.Infrastructure.Data;
using CourseService.Core.Interfaces;
using CourseService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Course Service API", 
        Version = "v1",
        Description = "E-Learning Platform - Course Management Service"
    });
});

// Database configuration
builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Message Queue
var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddSingleton<IMessageQueueService>(provider =>
    new RabbitMQService(
        rabbitMQSettings["Host"],
        rabbitMQSettings["Username"],
        rabbitMQSettings["Password"]
    ));

// Repositories
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICourseTopicRepository, CourseTopicRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
builder.Services.AddScoped<IUserProgressRepository, UserProgressRepository>();

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CourseDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Service API V1");
    });
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();