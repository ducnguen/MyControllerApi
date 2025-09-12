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
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories); 
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(CategoryDto requestDto)
    {
        var category = new Category
        {
            Name = requestDto.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpDelete("{id}")]
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