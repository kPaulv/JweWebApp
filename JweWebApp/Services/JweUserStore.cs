using JweWebApp.Data;
using JweWebApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JweWebApp.Services
{
    public class JweUserStore : UserStore<IdentityUser>
    {
        public JweUserStore(JweDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public async Task<RefreshTokens> GetRefreshToken(string userId)
        {
            var refreshTokenModel = await Context.FindAsync(typeof(RefreshTokens), userId);

            return (RefreshTokens)refreshTokenModel;
        }

        public async Task<bool> AddRefreshToken(RefreshTokens model)
        {
            var refreshTokenModel = await Context.FindAsync(typeof(RefreshTokens), model.UserId);

            if (refreshTokenModel != null)
            {
                Context.Remove(refreshTokenModel);
            }
            await Context.AddAsync(model);
            var result = await Context.SaveChangesAsync();
            return Convert.ToBoolean(result);
        }

        public async Task<bool> RevokeRefreshToken(string userId)
        {
            var refreshTokenModel = (RefreshTokens)await Context.FindAsync(typeof(RefreshTokens), userId);
            if (refreshTokenModel == null)
            {
                return false;
            }
            Context.Remove(refreshTokenModel);
            var result = await Context.SaveChangesAsync();
            return Convert.ToBoolean(result);
        }
    }
}
