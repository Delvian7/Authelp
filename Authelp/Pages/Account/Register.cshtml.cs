
using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Authelp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;
        public RegisterModel(AppDbContext db) { _db = db; }

        [BindProperty] public string? Name { get; set; }
        [BindProperty] public string? Email { get; set; }
        [BindProperty] public string? Phone { get; set; }
        [BindProperty] public string? Password { get; set; }
        [BindProperty] public string? Role { get; set; }
        [BindProperty] public string? Condition { get; set; } 
        [BindProperty] public string? ChildCondition { get; set; } 

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
            {
                ModelState.AddModelError("", "Name, Email, Password and Role are required.");
                return Page();
            }

            if (_db.Users.Any(u => u.Email == Email))
            {
                ModelState.AddModelError("", "Email already registered.");
                return Page();
            }

            var user = new User
            {
                Name = Name!,
                Email = Email!,
                Phone = Phone ?? "",
                Role = Role!,
                PasswordPlain = Password!,
                Contact = Condition ?? "" 
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            if (Role == "parent" && !string.IsNullOrWhiteSpace(ChildCondition))
            {
                _db.Children.Add(new Child
                {
                    ParentId = user.Id,
                    ChildName = "Child",
                    ChildCondition = ChildCondition!
                });
                await _db.SaveChangesAsync();
            }

            await SignInUser(user);
            TempData["RegisterSuccess"] = "Registration successful! Please log in.";
            return RedirectToPage("/Account/Login");
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, "AuthCookie");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("AuthCookie", principal);
        }
    }
}
