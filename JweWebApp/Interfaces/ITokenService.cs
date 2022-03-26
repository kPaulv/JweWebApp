using JweWebApp.Entities;
using JweWebApp.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JweWebApp.Interfaces
{
    public interface ITokenService
    {
        JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, JweConfiguration configuration);
        RefreshTokens GenerateRefreshToken(string userId, JweConfiguration configuration);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
