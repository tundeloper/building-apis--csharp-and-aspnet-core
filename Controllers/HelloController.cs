using Microsoft.AspNetCore.Mvc;

namespace MyApiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello from Controller!" });
        }
    }
}
