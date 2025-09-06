using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using UnictiveTest.AuthJwt;
using UnictiveTest.Data;
using UnictiveTest.Models;

namespace UnictiveTest.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;
        private readonly JwtOptions _opts;

        public LoginModel(AppDbContext db, JwtService jwt, IOptions<JwtOptions> opts)
        {
            _db = db;
            _jwt = jwt;
            _opts = opts.Value;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public string? Error { get; set; }

        public class LoginInput
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == Input.Email);
            if (user is null || !PasswordHasher.Verify(Input.Password, user.PasswordHash))
            {
                Error = "Invalid email or password";
                return Page();
            }

            var token = _jwt.CreateToken(user.Id, user.Email, user.Name);

            Response.Cookies.Append(_opts.CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return RedirectToPage("/Users/Index");
        }
    }
}
