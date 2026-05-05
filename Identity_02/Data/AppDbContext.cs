using Identity_02.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_02.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<AppUser> AppUsers { get; set; }
    }
}
