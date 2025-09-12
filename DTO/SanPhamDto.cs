namespace MyControllerApi.DTO;

public class SanPhamDto
{
    public int Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public bool TrangThai { get; set; }
    public int CategoryId {get;set;}
    
    public string CategoryName {get;set;} = string.Empty;
}