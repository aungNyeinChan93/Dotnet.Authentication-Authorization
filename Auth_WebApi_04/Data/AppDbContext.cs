using Auth_WebApi_04.Entity;
using Microsoft.EntityFrameworkCore;

namespace Auth_WebApi_04.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
        public DbSet<User> Users { get; set; }


    }
}
