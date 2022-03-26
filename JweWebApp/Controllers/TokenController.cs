using JweWebApp.Interfaces;
using JweWebApp.Model;
using JweWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace JweWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JweUserStore _userStore;
        private readonly IConfigurationService _configurationService;
        private readonly ITokenService _tokenService;

        public TokenController(UserManager<IdentityUser> userManager, JweUserStore userStore,
                                    IConfigurationService configurationService, ITokenService tokenService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _configurationService = configurationService;
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }
            string accessToken = tokenModel.AccessToken;
            string refreshToken = tokenModel.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);
            //RefreshToken with UserStore
            var refreshTokenModel = _userStore.GetRefreshToken(user.Id).Result;
            if (refreshTokenModel == null || refreshTokenModel.RefreshToken != refreshToken ||
                                             refreshTokenModel.RefreshTokenExpiryTime.ToUniversalTime() <= DateTime.Now.ToUniversalTime())
            {
                return BadRequest("Invalid client request");
            }

            var tokenConfig = _configurationService.GenerateConfiguration();
            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(_tokenService.GenerateAccessToken(principal.Claims, tokenConfig));
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id, tokenConfig);
            return new ObjectResult(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var result = await _userStore.RevokeRefreshToken(user.Id);
            if (!result)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
