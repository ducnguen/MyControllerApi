namespace MyControllerApi.DTO;

public class CreateSanPham
{
    public string Ten { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public bool TrangThai { get; set; }
    public int CategoryId {get;set;}
}