using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TechnicalTest.Data;
using TechnicalTest.Backend.Repositories.Interfaces;
using TechnicalTest.Backend.Repositories.Implementations;
using TechnicalTest.Backend.UnitsOfWork.Interfaces;
using TechnicalTest.Backend.UnitsOfWork.Implementations;
using TechnicalTest.Backend.Data;
using TechnicalTest.ClassLibrary.Entities.Users;
using TechnicalTest.ClassLibrary.Entities.Management;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Technical Test API", 
        Version = "v1" 
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5125", "http://localhost:5130", "https://localhost:7004", "http://localhost:5117")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));

builder.Services.AddScoped<IGenericUnitOfWork<User>, GenericUnitOfWork<User>>();
builder.Services.AddScoped<IGenericUnitOfWork<Hotel>, GenericUnitOfWork<Hotel>>();
builder.Services.AddScoped<IGenericUnitOfWork<Reservation>, GenericUnitOfWork<Reservation>>();

var app = builder.Build();

SeedData(app);

async void SeedData(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var seedDb = new SeedDb(context, scope.ServiceProvider);
        await seedDb.SeedAsync();
    }
}


app.UseCors(x => x
   .AllowAnyMethod()
   .AllowAnyHeader()
   .SetIsOriginAllowed(origin => true)
   .AllowCredentials());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
