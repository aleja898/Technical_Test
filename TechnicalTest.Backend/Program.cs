using Microsoft.EntityFrameworkCore;
using TechnicalTest.Backend.Data;
using TechnicalTest.Backend.Repositories.Interfaces;
using TechnicalTest.Backend.Repositories.Implementations;
using TechnicalTest.Backend.UnitsOfWork.Interfaces;
using TechnicalTest.Backend.UnitsOfWork.Implementations;
using TechnicalTest.ClassLibrary.Entities.Users;
using TechnicalTest.ClassLibrary.Entities.Management;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configurar ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar patrón Repository y UnitOfWork genéricos
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));

// Configurar servicios específicos para nuestras entidades
builder.Services.AddScoped<IGenericUnitOfWork<User>, GenericUnitOfWork<User>>();
builder.Services.AddScoped<IGenericUnitOfWork<Hotel>, GenericUnitOfWork<Hotel>>();
builder.Services.AddScoped<IGenericUnitOfWork<Reservation>, GenericUnitOfWork<Reservation>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
