using Microsoft.AspNetCore.Mvc.RazorPages;
using JweWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JweWebApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        [BindProperty]
        public Register RegisterViewModel { get; set; }

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = RegisterViewModel.Email,
                    Email = RegisterViewModel.Email
                };

                var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);
                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }

                foreach(var e in result.Errors)
                {
                    ModelState.AddModelError("", e.Description);
                }
            }

            return Page();
        }
    }
}
