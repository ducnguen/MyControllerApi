using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyControllerApi.Data;
using MyControllerApi.DTO;
using MyControllerApi.Models;

namespace MyControllerApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SanPhamController : ControllerBase
{
    private readonly DataContext _context;
    public SanPhamController(DataContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SanPhamDto>>> GetSanPham()
    {
        var sanPhams = await _context.SanPhams
            .Include(sp => sp.Category)
            .Select(sp => new SanPhamDto
            {
                Id = sp.Id,
                Ten = sp.Ten,
                Gia = sp.Gia,
                TrangThai = sp.TrangThai,
                CategoryId = sp.CategoryId,
                CategoryName = sp.Category!.Name
            })
            .ToListAsync();
        return Ok(sanPhams);;
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<SanPhamDto>> LaySanPham(int id)
    {
        var sanPham = await _context.SanPhams
            .Include(sp => sp.Category)
            .FirstOrDefaultAsync(sp => sp.Id == id);

        if (sanPham == null)
        {
            return NotFound();
        }

        // Chuyển đổi thủ công
        var sanPhamDto = new SanPhamDto
        {
            Id = sanPham.Id,
            Ten = sanPham.Ten,
            Gia = sanPham.Gia,
            TrangThai = sanPham.TrangThai,
            CategoryId = sanPham.CategoryId,
            CategoryName = sanPham.Category!.Name
        };

        return Ok(sanPhamDto);
    }   
    [HttpPost]
    public async Task<ActionResult<SanPhamDto>> PostSanPham(CreateSanPhamDto requestDto)
    {
        if (await _context.SanPhams.AnyAsync(c => c.Ten.ToLower() == requestDto.Ten.ToLower()))
        {
            // Nếu có, trả về lỗi 400 Bad Request
            return BadRequest("Tên danh mục này đã tồn tại.");
        }
        // 1. Lấy đối tượng Category từ CSDL (chỉ đi 1 lần)
        var category = await _context.Categories.FindAsync(requestDto.CategoryId);
        if (category == null)
        {
            return BadRequest("Category không hợp lệ.");
        }

        // 2. Tạo Model, gán cả CategoryId và đối tượng Category đã lấy được
        var newSanPham = new SanPham
        {
            Ten = requestDto.Ten,
            Gia = requestDto.Gia,
            TrangThai = requestDto.TrangThai,
            CategoryId = requestDto.CategoryId,
            Category = category // Gán đối tượng đã có sẵn
        };
    
        _context.SanPhams.Add(newSanPham);
        await _context.SaveChangesAsync();
    
        // 3. Bây giờ newSanPham.Category đã có sẵn, không cần LoadAsync() nữa
        var sanPhamDto = new SanPhamDto
        {
            Id = newSanPham.Id,
            Ten = newSanPham.Ten,
            Gia = newSanPham.Gia,
            TrangThai = newSanPham.TrangThai,
            CategoryId = newSanPham.CategoryId,
            CategoryName = newSanPham.Category.Name
        };

        // 4. Sửa lại tên action cho đúng
        return CreatedAtAction(nameof(LaySanPham), new { id = sanPhamDto.Id }, sanPhamDto);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSanPham(int id, UpdateSanPhamDto dto)
    {
        var sanPhamInDb = await _context.SanPhams.FindAsync(id);

        if (sanPhamInDb == null)
        {
            return NotFound();
        }

        // Cập nhật các thuộc tính từ DTO
        sanPhamInDb.Ten = dto.Ten;
        sanPhamInDb.Gia = dto.Gia;
        sanPhamInDb.TrangThai = dto.TrangThai;
        sanPhamInDb.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteSanPham(int id)
    {
        var sanPham = await _context.SanPhams.FindAsync(id);
        if (sanPham == null)
        {
            return NotFound();
        }
        _context.SanPhams.Remove(sanPham);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    
}