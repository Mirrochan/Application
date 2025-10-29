using Application;
using Application.Abstractions;
using Application.Services;
using Application.Validators;
using Event_Management_System.Extensions;
using Event_Management_System.Middleware;
using FluentValidation;
using Infrastructure;
using Infrastructure.Jwt;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Infrastructure.AI;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Configuration.AddJsonFile("appsettings.Develop.json", optional: true, reloadOnChange: true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Management API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<EventManagmentSystemDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<GroqApiSettings>(builder.Configuration.GetSection("GroqApi"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IAiConversationRepository, AiConversationRepository>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAiAssistantService, AiAssistantService>();


builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();


builder.Services.AddHttpClient<IAiProvider, GroqAiProvider>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["GroqApi:ApiKey"];
    var baseUrl = config["GroqApi:BaseUrl"];

    if (string.IsNullOrEmpty(apiKey))
        throw new InvalidOperationException("Groq API Key is not configured");
    if (string.IsNullOrEmpty(baseUrl))
        throw new InvalidOperationException("Groq BaseUrl is not configured");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    client.DefaultRequestHeaders.Add("User-Agent", "EventManagementSystem/1.0");

    client.Timeout = TimeSpan.FromSeconds(60);
});


builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEventRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateEventRequestValidator>();

builder.Services.AddJwtAuthentication(builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddLogging();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventManagmentSystemDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    await DbSeeder.SeedAsync(context);
}

app.Run();