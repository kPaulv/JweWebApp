using JweWebApp.Entities;
using JweWebApp.Interfaces;
using JweWebApp.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JweWebApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, JweConfiguration configuration)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Secret));
            var authEncryptingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.EncryptionKey));
            var authClaims = claims;

            var accessToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
                issuer: configuration.ValidIssuer,
                audience: configuration.ValidAudience,
                subject: new ClaimsIdentity(authClaims),
                notBefore: DateTime.Now.ToUniversalTime(),
                expires: DateTime.Now.ToUniversalTime().AddHours(configuration.AccessTokenExpiration),
                issuedAt: DateTime.Now,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                encryptingCredentials: new EncryptingCredentials(authEncryptingKey, JwtConstants.DirectKeyUseAlg, 
                                                            SecurityAlgorithms.Aes256CbcHmacSha512));
            /*var accessToken = new JwtSecurityToken(
                    issuer: configuration.ValidIssuer,
                    audience: configuration.ValidAudience,
                    expires: DateTime.Now.ToUniversalTime().AddHours(configuration.AccessTokenExpiration),
                    claims: authClaims,
                     
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );*/
            return accessToken;
        }
        public RefreshTokens GenerateRefreshToken(string userId, JweConfiguration configuration)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return new RefreshTokens
                {
                    UserId = userId,
                    RefreshToken = Convert.ToBase64String(randomNumber),
                    RefreshTokenExpiryTime = DateTime.Now.ToUniversalTime().AddDays(configuration.RefreshTokenExpiration)
                };
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
