using Infrastructure;
using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using API.Exceptions;
using System.Security.Claims;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? throw new Exception("DB_NAME missing");
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? throw new Exception("DB_USER missing");
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new Exception("DB_PASSWORD missing");

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass};";

var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new Exception("JWT_ISSUER missing");
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new Exception("JWT_AUDIENCE missing");

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT_KEY missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(builder.Configuration, connectionString);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//EXCEPCIONES - REGISTRAR LOS HANDLERS EN EL ORDEN CORRECTO (DE MÁS ESPECÍFICOS A MÁS GENERALES)
//-------------------------------------------------------------------------------------------------
builder.Services.AddSingleton<IExceptionHandler, ValidationExceptionHandler>();
builder.Services.AddSingleton<IExceptionHandler, UnauthorizedExceptionHandler>();
builder.Services.AddSingleton<IExceptionHandler, ForbiddenExceptionHandler>();
//siempre el GlobalExceptionHandler va al final, porque es el "catch-all" que maneja cualquier excepción no manejada por los anteriores
builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();
//-------------------------------------------------------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebApp", policy =>
    {
        policy.AllowAnyOrigin() // En producción pondríamos la URL del frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// --- CONFIGURACIÓN DE SWAGGER PARA JWT ---
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inka Real Estate API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Por favor ingresa el token así: 'Bearer {tu_token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
// -----------------------------------------

var app = builder.Build();

app.UseCors("WebApp");

app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
app.Run();