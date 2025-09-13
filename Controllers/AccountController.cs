using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MyControllerApi.DTO;
using MyControllerApi.Models;
using MyControllerApi.Services;

namespace MyControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly TokenService _tokenService;

    public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto requestDto)
    {
        var user = new AppUser
        {
            UserName = requestDto.Username,
            Email = requestDto.Email
        };

        // Dùng UserManager để tạo người dùng mới
        var result = await _userManager.CreateAsync(user, requestDto.Password);

        if (result.Succeeded)
        {
            return Ok("Người dùng đã được tạo thành công!");
        }

        // Nếu thất bại, trả về danh sách lỗi
        return BadRequest(result.Errors);
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto requestDto)
    {
        var user = await _userManager.FindByNameAsync(requestDto.Username);
        if (user == null) return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, requestDto.Password, false);
    
        if (result.Succeeded)
        {
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        return Unauthorized();
    }
}