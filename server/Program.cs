using System.Reflection;
using Overtake.Interfaces;
using Overtake.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Overtake;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHostedService<RaceTriggerService>();
        builder.Services.AddHttpClient();

        // Swagger/OpenAPI configuration
        builder.Services.AddSwaggerGen(options =>
        {
            // Enables loading XML docs for OpenAPI
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        // CORS policy configuration
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        // JWT Authentication configuration
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
        });

        // Database initialization and dependency injection
        builder.Services.AddHostedService<DbInitService>();
        builder.Services.AddControllers();
        builder.Services.AddSingleton<IDatabase, PostgresDatabase>();

        // Register email service for dependency injection
        builder.Services.AddScoped<IEmailService, EmailService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
