using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UnictiveTest.AuthJwt;
using UnictiveTest.Data;
using UnictiveTest.Models;

namespace UnictiveTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;
        private readonly JwtOptions _opts;

        public AuthController(AppDbContext db, JwtService jwt, IOptions<JwtOptions> opts)
        {
            _db = db;
            _jwt = jwt;
            _opts = opts.Value;
        }

        public record LoginRequest(string Email, string Password);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _db.Users.Include(u => u.Hobbies).SingleOrDefaultAsync(u => u.Email == req.Email);
            if (user is null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _jwt.CreateToken(user.Id, user.Email, user.Name);

            Response.Cookies.Append(_opts.CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return Ok(new { token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(_opts.CookieName);
            return Ok(new { message = "Logged out" });
        }
    }
}
