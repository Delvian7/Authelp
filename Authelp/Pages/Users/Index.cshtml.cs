using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Authelp.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public IndexModel(AppDbContext db) { _db = db; }

        public List<User> Parents { get; set; } = new();
        public List<User> Helpers { get; set; } = new();

        public async Task OnGetAsync()
        {
            Parents = await _db.Users.Where(u => u.Role == "parent").ToListAsync();
            Helpers = await _db.Users.Where(u => u.Role == "helper").ToListAsync();
        }
    }
}
