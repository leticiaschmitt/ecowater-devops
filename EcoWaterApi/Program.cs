using System.Text;
using EcoWaterApi.Data;
using EcoWaterApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcoWaterApi",
        Version = "1.0",
        Description = "API para monitoramento inteligente de consumo de água, alertas de desperdício e relatórios ambientais."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite o token JWT gerado no endpoint /api/auth/login."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<SensorService>();
builder.Services.AddScoped<ConsumptionService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<ReportService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? string.Empty;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Health check usado pelo Docker (HEALTHCHECK) e pelos smoke tests do pipeline CI/CD
builder.Services.AddHealthChecks();

var app = builder.Build();

// Aplica as migrations automaticamente ao subir (containers, staging e produção).
// O SQL Server pode demorar a aceitar conexões, por isso há novas tentativas.
if (app.Configuration.GetValue("Database:ApplyMigrations", true))
{
    var maxAttempts = app.Configuration.GetValue("Database:MigrationRetries", 10);
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            app.Logger.LogInformation("Migrations aplicadas com sucesso (tentativa {Attempt}).", attempt);
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            app.Logger.LogWarning(ex, "Banco indisponível (tentativa {Attempt}/{Max}). Nova tentativa em 5s...", attempt, maxAttempts);
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// Dentro do container o TLS é tratado fora da aplicação (proxy/load balancer)
if (!app.Configuration.GetValue<bool>("DisableHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }