using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UnictiveTest.Data;
using UnictiveTest.Models;

namespace UnictiveTest.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;
        public CreateModel(AppDbContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public List<string> HobbyInputs { get; set; } = new();

        public class InputModel
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }

        public void OnGet()
        {
            if (!HobbyInputs.Any())
            {
                HobbyInputs.Add("");
            }
        }

        public IActionResult OnPostAddHobby()
        {
            if (HobbyInputs == null)
                HobbyInputs = new List<string>();

            HobbyInputs.Add(""); // tambahkan field kosong
            return Page();
        }

        public IActionResult OnPostRemoveHobby(int index)
        {
            if (HobbyInputs != null && index >= 0 && index < HobbyInputs.Count)
            {
                HobbyInputs.RemoveAt(index);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (await _db.Users.AnyAsync(u => u.Email == Input.Email))
            {
                ModelState.AddModelError("Input.Email", "Email sudah digunakan.");
                return Page();
            }

            var user = new User
            {
                Name = Input.Name,
                Email = Input.Email,
                PasswordHash = PasswordHasher.Hash(Input.Password),
                Hobbies = HobbyInputs
                    .Where(h => !string.IsNullOrWhiteSpace(h))
                    .Select(h => new Hobby { Name = h.Trim() })
                    .ToList()
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
