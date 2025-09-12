using Microsoft.EntityFrameworkCore;
using MyControllerApi.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddControllers(); // << Dòng này bạn đã có
// builder.Services.AddControllers().AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
// });
// Thay thế AddOpenApi() bằng 2 dòng tiêu chuẩn này:
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    // Giả sử bạn dùng PostgreSQL
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    // Nếu bạn dùng SQL Server, hãy dùng options.UseSqlServer(...)
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Thay thế MapOpenApi() bằng 2 dòng tiêu chuẩn này:
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers(); // << Dòng này bạn đã có

app.Run();