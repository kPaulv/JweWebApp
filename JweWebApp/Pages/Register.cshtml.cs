using Microsoft.AspNetCore.Mvc.RazorPages;
using JweWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using JweWebApp.Model;

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
                var userExists = await _userManager.FindByEmailAsync(RegisterViewModel.Email);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

                var user = new IdentityUser()
                {
                    UserName = RegisterViewModel.Email,
                    Email = RegisterViewModel.Email
                };

                var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);
                /*if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }

                foreach(var e in result.Errors)
                {
                    ModelState.AddModelError("", e.Description);
                }*/

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                await _signInManager.SignInAsync(user, false);
                //return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                return RedirectToPage("Index");



            }

            return Page();
        }
    }
}
