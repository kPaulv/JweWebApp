using JweWebApp.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JweWebApp.Data
{
    public class JweDbContext : IdentityDbContext
    {
        public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }
        public JweDbContext(DbContextOptions<JweDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshTokens>().HasKey(u => u.UserId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
