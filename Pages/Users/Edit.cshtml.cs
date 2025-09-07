using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UnictiveTest.Data;
using UnictiveTest.Models;

namespace UnictiveTest.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;
        public EditModel(AppDbContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public List<string> HobbyInputs { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string? NewPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _db.Users
                .Include(u => u.Hobbies)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            Input = new InputModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            HobbyInputs = user.Hobbies.Select(h => h.Name).ToList();

            if (!HobbyInputs.Any())
                HobbyInputs.Add(""); // default satu field

            return Page();
        }

        public IActionResult OnPostAddHobby()
        {
            if (HobbyInputs == null)
                HobbyInputs = new List<string>();

            HobbyInputs.Add("");
            return Page();
        }

        public IActionResult OnPostRemoveHobby(int index)
        {
            if (HobbyInputs != null && index >= 0 && index < HobbyInputs.Count)
                HobbyInputs.RemoveAt(index);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _db.Users
                .Include(u => u.Hobbies)
                .FirstOrDefaultAsync(u => u.Id == Input.Id);

            if (user == null) return NotFound();

            user.Name = Input.Name;
            user.Email = Input.Email;

            if (!string.IsNullOrWhiteSpace(Input.NewPassword))
            {
                user.PasswordHash = PasswordHasher.Hash(Input.NewPassword);
            }

            user.Hobbies.Clear();
            foreach (var hobby in HobbyInputs.Where(h => !string.IsNullOrWhiteSpace(h)))
            {
                user.Hobbies.Add(new Hobby { Name = hobby.Trim() });
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
