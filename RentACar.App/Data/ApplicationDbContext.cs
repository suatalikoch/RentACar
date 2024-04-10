using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.App.Domain;

namespace RentACar.App.Data
{
    public class ApplicationDbContext : IdentityDbContext<RentACarUser>
    {
        public DbSet<Car> Cars { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
