using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
      private readonly IAuthService _authService;

        public AuthController(IAuthService authservice)
        {
                _authService = authservice;
        }


        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserModel model)
        {
            var token = _authService.Authenticate(model.Username, model.Password);

            if (token == null)
                return Unauthorized(new { message = "Kullanıcı adı veya parola geçersiz." });

            return Ok(new { Token = token });
        }
   
    }
}
