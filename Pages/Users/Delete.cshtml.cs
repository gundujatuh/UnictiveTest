using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UnictiveTest.Data;
using UnictiveTest.Models;

namespace UnictiveTest.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _db;
        public DeleteModel(AppDbContext db) => _db = db;

        public User? User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            User = await _db.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (User is null) return RedirectToPage("Index");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}
