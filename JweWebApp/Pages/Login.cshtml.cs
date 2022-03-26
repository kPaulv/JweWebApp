using JweWebApp.Interfaces;
using JweWebApp.Model;
using JweWebApp.Services;
using JweWebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JweWebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfigurationService _configurationService;
        private readonly ITokenService _tokenService;
        private readonly JweUserStore _userStore;

        [BindProperty]
        public Login LoginViewModel { get; set; }
        public LoginModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
                            IConfigurationService configurationService, ITokenService tokenService,
                            JweUserStore userStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configurationService = configurationService;
            _tokenService = tokenService;
            _userStore = userStore;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(LoginViewModel.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, LoginViewModel.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var tokenConfig = _configurationService.GenerateConfiguration();
                    var token = _tokenService.GenerateAccessToken(authClaims, tokenConfig);
                    var refreshToken = _tokenService.GenerateRefreshToken(user.Id, tokenConfig);
                    await _userStore.AddRefreshToken(refreshToken);

                    var result = await _signInManager.PasswordSignInAsync(LoginViewModel.Email, LoginViewModel.Password, LoginViewModel.RememberMe, false);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return StatusCode(StatusCodes.Status200OK, new
                        {
                            Status = "Success",
                            Message = "User logged in.",
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            refreshToken = refreshToken
                        });
                    }
                    

                    /*return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        refreshToken = refreshToken
                    });*/
                }

                /*var result = await _signInManager.PasswordSignInAsync(LoginViewModel.Email, LoginViewModel.Password, LoginViewModel.RememberMe, false);
                if (result.Succeeded)
                {
                    if (returnUrl == null || returnUrl == "/")
                    {
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        return RedirectToPage(returnUrl);
                    }
                }

                ModelState.AddModelError("", "Username or Password is incorrect.");*/
            }

            return Page();
        }
    }
}
