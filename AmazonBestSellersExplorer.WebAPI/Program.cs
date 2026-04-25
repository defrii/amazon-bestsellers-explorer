using Microsoft.EntityFrameworkCore;
using AmazonBestSellersExplorer.WebAPI.Data;
using AmazonBestSellersExplorer.WebAPI.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// In-memory EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AmazonBestSellers_InMemory"));

// Use ASP.NET Core Identity password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowAngular");
app.Run();