using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MyControllerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MyControllerApi.Data;

public class DataContext : IdentityDbContext<AppUser>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<SanPham> SanPhams { get; set; }
    public DbSet<Category> Categories { get; set; }
}