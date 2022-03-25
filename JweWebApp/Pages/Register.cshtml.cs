using Microsoft.AspNetCore.Mvc.RazorPages;
using JweWebApp.ViewModels;

namespace JweWebApp.Pages
{
    public class RegisterModel : PageModel
    {
        public Register RegisterViewModel { get; set; }
        public void OnGet()
        {
        }
    }
}
