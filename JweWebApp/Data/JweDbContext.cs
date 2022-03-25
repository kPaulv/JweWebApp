﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JweWebApp.Data
{
    public class JweDbContext : IdentityDbContext
    {
        public JweDbContext(DbContextOptions<JweDbContext> options) : base(options)
        {

        }
    }
}