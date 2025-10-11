using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Database;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NeonTechDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Registramos el servicio correctamente (ya estaba, se mantiene)
builder.Services.AddScoped<IServiceReservations, ServiceReservations>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
