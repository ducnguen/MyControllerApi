using Microsoft.AspNetCore.Mvc;

namespace MyControllerApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class XinChaoController : ControllerBase
{
    [HttpGet]
    public IActionResult XinChao()
    {
        var traVe = new
        {
            tinNhan = "Bai code .net dau tien cua toi",
            thoiGian = DateTime.UtcNow
        };
        return Ok(traVe);
    }
}