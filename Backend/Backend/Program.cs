using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Objects.Models;
using Backend.Implementations.Logic;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using static Backend.Infraestructure.Objects.Models.Tablas;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NeonTechDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMetadataService, MetadataService>();
builder.Services.AddScoped<MetadataRawService>();
builder.Services.AddScoped<IMetadataService, MetadataService>();
builder.Services.AddControllers();

// Importation of each file with endpoints
builder.Services.AddScoped<IConnectionService, Connection>();
builder.Services.AddScoped<IUsers, Login>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
