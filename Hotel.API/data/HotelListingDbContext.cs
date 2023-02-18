using Hotel.API.Data;
using Hotel.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace HotelListing.API.Data
{
    public class HotelListingDbContext: IdentityDbContext<ApiUser>
    {

        public HotelListingDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Inn> HotelListings { get; set; }

        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.ApplyConfiguration(new CountryConfiguration());

            modelBuilder.ApplyConfiguration(new HotelConfiguration());
        }

    }
}
