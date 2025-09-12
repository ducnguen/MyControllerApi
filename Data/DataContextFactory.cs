using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyControllerApi.Data;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        // 1. Tạo một đối tượng cấu hình để đọc file appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // 2. Tạo một đối tượng DbContextOptionsBuilder
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        // 3. Lấy chuỗi kết nối từ file cấu hình
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 4. Chỉ định sử dụng PostgreSQL với chuỗi kết nối đã lấy
        optionsBuilder.UseNpgsql(connectionString);

        // 5. Trả về một đối tượng DataContext đã được cấu hình
        return new DataContext(optionsBuilder.Options);
    }
}