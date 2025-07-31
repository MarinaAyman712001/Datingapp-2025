using API.Data;
using API.entities;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ربط قاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     var tokenKey = builder.Configuration["TokenKey"]
     ?? throw new Exception("Token Key - prgram.cs");
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
          ValidateIssuer = false,
         ValidateAudience = false
     };
 });

var app = builder.Build();

// 🧠 تشغيل المايجريشن + إضافة مستخدم بكلمة مرور مشفّرة
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
context.Database.Migrate();

// ✅ إضافة مستخدم واحد بكلمة مرور مشفّرة
if (!context.Users.Any())
{
    using var hmac = new HMACSHA512();
    var password = "123456"; // الباسورد الحقيقي

    var user = new AppUser
    {
        DisplayName = "marina",
        Email = "marina@example.com",
        passwardHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
        passwardSalt = hmac.Key
    };

    context.Users.Add(user);
    context.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
