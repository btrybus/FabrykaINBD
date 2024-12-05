using FabrykaINBD.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FabrykaINBD.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hala> Hale { get; set; }
        public DbSet<Maszyna> Maszyny { get; set; }
        public DbSet<Operator> Operatorzy { get; set; }
    }
}
