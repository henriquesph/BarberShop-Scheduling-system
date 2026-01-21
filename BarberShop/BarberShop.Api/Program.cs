//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();

//var app = builder.Build();

//// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using BarberShop.Domain.Interfaces;
using BarberShop.Domain.Services;
using BarberShop.Infrastructure;
using BarberShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add the Database (In-Memory)
builder.Services.AddDbContext<BarberDbContext>(options =>
    options.UseInMemoryDatabase("BarberShopDb"));

// 2. Register the Repository
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// 3. Register the Service
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
