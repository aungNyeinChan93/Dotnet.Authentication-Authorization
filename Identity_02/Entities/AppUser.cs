using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity_02.Entities
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName ="nvarchar(255)")]
        public string FullName { get; set; } = string.Empty;
    }
}
