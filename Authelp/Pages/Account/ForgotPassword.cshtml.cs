using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Authelp.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ForgotPasswordModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        public string? Message { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("Email", "Email is required.");
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null)
            {
                Message = "If the email exists, a password reset link has been sent.";
                return Page(); 
            }

            
            user.PasswordResetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); 

            await _context.SaveChangesAsync();

            
            

            Message = "If the email exists, a password reset link has been sent.";
            return Page();
        }
    }
}
