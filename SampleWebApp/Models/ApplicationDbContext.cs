using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SampleWebApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {     

        public ApplicationDbContext()
            : base("SampleWebAppDb", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}