using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using AmazonBestSellersExplorer.WebAPI.Data;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Repositories;
using AmazonBestSellersExplorer.WebAPI.Services;
using AmazonBestSellersExplorer.WebAPI.Services.API;
using AmazonBestSellersExplorer.WebAPI.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // adres klienta web
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key nie został skonfigurowany.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

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
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = JwtRegisteredClaimNames.Name
        };
    });

builder.Services.AddScoped<IFavoriteProductRepository, FavoriteProductRepository>();
builder.Services.AddScoped<IFavoriteProductService, FavoriteProductService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var useExampleAmazonApi = builder.Configuration.GetValue<bool>("Amazon:UseExampleFile");
if (useExampleAmazonApi)
{
    builder.Services.AddScoped<IAmazonApiService, ExampleAmazonApiService>();
}
else
{
    builder.Services.AddScoped<IAmazonApiService, AmazonApiService>();
}

builder.Services.AddControllers();
builder.Services.AddHttpClient();
var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();