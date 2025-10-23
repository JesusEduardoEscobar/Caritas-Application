using Backend.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();       // Elimina los providers predeterminados
builder.Logging.AddConsole();           // Muestra logs en consola
builder.Logging.AddDebug();             // Muestra logs en la ventana de Debug de Visual Studio
builder.Logging.SetMinimumLevel(LogLevel.Warning); // Nivel mínimo

// Add services to the container.
builder.Services.AddDbContext<NeonTechDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Allow the frontend 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddScoped<IQrService, QrService>();

builder.Services.AddScoped<IAuthenticator, Authenticator>();
builder.Services.AddScoped<IUsers, Login>();

builder.Services.AddScoped<IShelters, SheltersManager>();
builder.Services.AddScoped<IServices, ServicesManager>();
builder.Services.AddScoped<IShelterServices, ShelterServicesManager>();
builder.Services.AddScoped<IServiceReservations, ServiceReservationsManager>();

builder.Services.AddScoped<IBeds, BedsManager>();
builder.Services.AddScoped<IReservations, ReservationsManager>();
builder.Services.AddScoped<IBedReservations, BedReservations>(); // remover más tarde si es posible

builder.Services.AddScoped<ICars, CarsManager>();
builder.Services.AddScoped<ITransportRequests, TransportRequestsManager>();


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
        ValidIssuer = "tuapp",
        ValidAudience = "tuapp",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("clave_secreta_muy_larga_123456789")),
        ClockSkew = TimeSpan.Zero
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums();

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Configuración JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de su token JWT. Ejemplo: Bearer {token}"
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
