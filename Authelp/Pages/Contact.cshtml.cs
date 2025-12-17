using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Authelp.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            
            TempData["Success"] = "Your message has been submitted.";

            return Page();
        }
    }
}
