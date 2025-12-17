using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Authelp.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task OnGet()
        {
            await HttpContext.SignOutAsync("AuthCookie"); 
            Response.Redirect("/Account/Login");
        }
    }
}
