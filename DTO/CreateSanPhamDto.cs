using System.ComponentModel.DataAnnotations;

namespace MyControllerApi.DTO;

public class CreateSanPhamDto
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [StringLength(100, ErrorMessage = "Tên sản phẩm không được dài quá 100 ký tự")]
    public string Ten { get; set; } = string.Empty;
    
    [Range(1, 1000000000, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
    public decimal Gia { get; set; }
    public bool TrangThai { get; set; }
    
    [Required]
    public int CategoryId {get;set;}
}