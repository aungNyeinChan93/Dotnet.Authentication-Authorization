using Auth_WebApi_03.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth_WebApi_03.Data
{
    public class AppdbContext(DbContextOptions<AppdbContext> options):DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
