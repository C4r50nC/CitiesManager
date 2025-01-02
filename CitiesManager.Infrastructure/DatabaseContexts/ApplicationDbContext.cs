using CitiesManager.Core.Entities;
using CitiesManager.Core.Identities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public ApplicationDbContext() { }

        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().HasData(new City()
            {
                CityId = new Guid("CECCCFD4-B6DE-4C23-BD5A-B835915F1205"),
                CityName = "New York",
            });
            modelBuilder.Entity<City>().HasData(new City()
            {
                CityId = new Guid("6361A112-1D1D-41E8-82F0-BBDD966C45F3"),
                CityName = "London",
            });
        }
    }
}
