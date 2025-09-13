using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyControllerApi.Data;
using MyControllerApi.DTO;
using MyControllerApi.Models;

namespace MyControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly DataContext _context;

    public CategoriesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        return await _context.Categories
            .Select(c => new CategoryDto{Id = c.Id, Name = c.Name})
            .ToListAsync<CategoryDto>();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }
        var categoryDto = new CategoryDto{Id = category.Id, Name = category.Name};
        return Ok(categoryDto);
    }
    
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> PostCategory(CategoryForCreationDto requestDto) // Đổi tên DTO cho rõ nghĩa
    {
        if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == requestDto.Name.ToLower()))
        {
            // Nếu có, trả về lỗi 400 Bad Request
            return BadRequest("Tên danh mục này đã tồn tại.");
        }
        var category = new Category { Name = requestDto.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name };

        // CreatedAtAction nên trả về DTO
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryDto);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<Category>> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}