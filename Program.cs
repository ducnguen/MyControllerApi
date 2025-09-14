using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyControllerApi.Data;
using MyControllerApi.Models;
using MyControllerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Phần 1: Đăng ký các dịch vụ ---
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();

// Cấu hình Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

// Cấu hình DbContext
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Cấu hình Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Cấu hình Authentication

var signingKeyBase64Url = builder.Configuration["Jwt:SigningKey"] 
                          ?? throw new InvalidOperationException("JWT SigningKey missing");
byte[] keyBytes;
{
    string s = signingKeyBase64Url.Trim().Replace('-', '+').Replace('_', '/');
    switch (s.Length % 4) { case 2: s += "=="; break; case 3: s += "="; break; }
    keyBytes = Convert.FromBase64String(s);
}
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)  // reduce during debug to see exact expiration issues
        };
    });

// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


// Đăng ký TokenService
builder.Services.AddScoped<TokenService>();


// --- Xây dựng ứng dụng ---
var app = builder.Build();

app.UseCors("AllowReactApp");
// --- Cấu hình pipeline ---
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