using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication_Authorization.WebApi_01.Data
{
    public class AppDbContext :IdentityDbContext
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
           
        }
    }
}
