using Authelp.Data;
using Authelp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Authelp.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly AppDbContext _context;

        public ResetPasswordModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Token { get; set; } = "";

        [BindProperty]
        public string NewPassword { get; set; } = "";

        [BindProperty]
        public string ConfirmPassword { get; set; } = "";

        public string? Message { get; set; }

        public IActionResult OnGet(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Message = "Invalid password reset token.";
                return Page();
            }

            Token = token;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return Page();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.PasswordResetToken == Token &&
                    u.PasswordResetTokenExpiry > DateTime.UtcNow
                );

            if (user == null)
            {
                Message = "Invalid or expired token.";
                return Page();
            }

            
            user.PasswordPlain = NewPassword;

            
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your password has been reset successfully. Please log in.";
            return RedirectToPage("/Account/Login");
        }
    }
}
