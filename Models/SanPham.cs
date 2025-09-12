namespace MyControllerApi.Models;

public class SanPham
{
    public int Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public bool TrangThai { get; set; }
    
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}