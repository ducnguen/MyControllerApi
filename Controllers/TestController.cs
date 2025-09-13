using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult GetPublicData()
    {
        return Ok("Đây là dữ liệu công khai, ai cũng xem được.");
    }

    [HttpGet("secure")]
    [Authorize] // << KHÓA ENDPOINT NÀY LẠI
    public IActionResult GetSecureData()
    {
        // Để tạo token giả để test, bạn có thể vào trang jwt.io
        // và dùng key "day la mot cai khoa bi mat sieu dai va an toan"
        return Ok("Đây là dữ liệu bí mật, chỉ người có token mới xem được!");
    }
}