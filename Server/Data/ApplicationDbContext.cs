using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using EFBlazorBasics_Wasm.Shared;

namespace EFBlazorBasics_Wasm.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Activity> Activitys { get; set; }
        public DbSet<Helper> Helpers { get; set; }
        public DbSet<Round> Rounds { get; set; }

    }
}
