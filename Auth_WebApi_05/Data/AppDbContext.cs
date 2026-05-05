using Microsoft.EntityFrameworkCore;

namespace Auth_WebApi_05.Data
{
    public class AppDbContext (DbContextOptions<AppDbContext> options):DbContext(options)
    {
        //public DbSet<> MyProperty { get; set; }
    }
}
