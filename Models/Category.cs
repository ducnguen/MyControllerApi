namespace MyControllerApi.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    
    public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}