using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Authelp.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        public LoginModel(AppDbContext db) => _db = db;

        [BindProperty] public string? Email { get; set; }
        [BindProperty] public string? Password { get; set; }

        public string? LoginError { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                LoginError = "Email and Password required.";
                return Page();
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                LoginError = "Account not found.";
                return Page();
            }

            if (user.PasswordPlain != Password)
            {
                LoginError = "Incorrect password.";
                return Page();
            }







            _db.LoginLogs.Add(new LoginLog { UserId = user.Id, LoginTime = DateTime.UtcNow });
            await _db.SaveChangesAsync();

       
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("DisplayName", user.Name)

            };

            var identity = new ClaimsIdentity(claims, "AuthCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
     "AuthCookie",
     principal,
     new AuthenticationProperties
     {
         IsPersistent = false  
     }
 );


            return RedirectToPage("/Index");
        }
    }
}
