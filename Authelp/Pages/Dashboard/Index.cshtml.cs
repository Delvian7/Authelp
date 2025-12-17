using System.Security.Claims;
using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Authelp.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public User? UserProfile { get; set; }
        public List<Child> Children { get; set; } = new();

        
        [BindProperty] public string? Name { get; set; }
        [BindProperty] public string? Email { get; set; }
        [BindProperty] public string? Phone { get; set; }
        [BindProperty] public string? Password { get; set; }

        [BindProperty]
        public string? ChildStory { get; set; }



        [BindProperty] public string? NewChildName { get; set; }
        [BindProperty] public string? NewChildCondition { get; set; }

        
        [BindProperty]
        public IFormFile? ProfileUpload { get; set; }


        
        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToPage("/Account/Login");

            UserProfile = await _db.Users
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (UserProfile == null)
                return RedirectToPage("/Account/Logout");

            
            Name = UserProfile.Name;
            Email = UserProfile.Email;
            Phone = UserProfile.Phone;
            Password = UserProfile.PasswordPlain;

            
            Children = await _db.Children
                .Where(c => c.ParentId == userId)
                .ToListAsync();

            return Page();
        }


        
       
        
        public async Task<IActionResult> OnPostUploadPhotoAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToPage("/Account/Login");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToPage("/Account/Logout");

            if (ProfileUpload != null && ProfileUpload.Length > 0)
            {
                
                var uploadDir = Path.Combine("wwwroot", "images", "users");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                
                var fileName = $"{Guid.NewGuid()}_{ProfileUpload.FileName}";
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ProfileUpload.CopyToAsync(stream);
                }

                
                user.ProfileImage = $"/images/users/{fileName}";
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        
        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToPage("/Account/Login");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToPage("/Account/Logout");

            user.Name = Name ?? user.Name;
            user.Email = Email ?? user.Email;
            user.Phone = Phone ?? user.Phone;

            if (!string.IsNullOrWhiteSpace(Password))
                user.PasswordPlain = Password;

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }


        
        public async Task<IActionResult> OnPostAddChildAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToPage("/Account/Login");

            if (string.IsNullOrWhiteSpace(NewChildName) || string.IsNullOrWhiteSpace(NewChildCondition))
            {
                ModelState.AddModelError("", "Child name and condition are required.");
                await OnGetAsync();
                return Page();
            }

            var child = new Child
            {
                ChildName = NewChildName,
                ChildCondition = NewChildCondition,
                ChildStory = ChildStory,
                ParentId = userId
            };

            _db.Children.Add(child);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }








        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToPage("/Account/Login");

            var user = await _db.Users
                .Include(u => u.Children)  
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToPage("/Account/Logout");

            
            _db.Children.RemoveRange(user.Children);

            
            _db.Users.Remove(user);

            await _db.SaveChangesAsync();

            
            await HttpContext.SignOutAsync();

            return RedirectToPage("/Account/Login");
        }






       





        public async Task<IActionResult> OnPostUpdateChildAsync(int ChildId, string ChildName, string ChildCondition)
        {
            var child = await _db.Children.FirstOrDefaultAsync(c => c.Id == ChildId);
            if (child == null) return NotFound();

            child.ChildName = ChildName;
            child.ChildCondition = ChildCondition;
            child.ChildStory = ChildStory;

            await _db.SaveChangesAsync();
            return RedirectToPage();
        }


        
        public async Task<IActionResult> OnPostDeleteChildAsync(int childId)
        {
            var child = await _db.Children.FirstOrDefaultAsync(c => c.Id == childId);
            if (child != null)
            {
                _db.Children.Remove(child);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
