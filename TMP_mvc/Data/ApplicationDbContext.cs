using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMP_mvc.Models;

namespace TMP_mvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<UserCity> UserCities { get; set; }
        public DbSet<TemperatureHistory> TemperatureHistories { get; set; }

        public DbSet<Comment> Comments { get; set; }

    }
}
