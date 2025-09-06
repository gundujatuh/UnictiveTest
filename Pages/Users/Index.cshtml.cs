using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UnictiveTest.Data;
using UnictiveTest.Models;

namespace UnictiveTest.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db) => _db = db;

        public List<User> Users { get; set; } = new();

        public async Task OnGetAsync(int page = 1, int pageSize = 5)
        {
            Users = await _db.Users.Include(u => u.Hobbies).OrderBy(u => u.Id).ToListAsync();
        }
    }
}
