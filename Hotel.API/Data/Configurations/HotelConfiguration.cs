using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.API.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Inn>
    {
        public void Configure(EntityTypeBuilder<Inn> builder)
        {
            builder.HasData(

                 new Inn
                 {
                     Id = 1,
                     Name = "Sandals Resort and Spa",
                     Address = "Negril",
                     CountryId = 1,
                     Rating = 4.5

                 },
                new Inn
                {
                    Id = 2,
                    Name = "Comfort Suites",
                    Address = "George Town",
                    CountryId = 3,
                    Rating = 4.5

                },
                new Inn
                {
                    Id = 3,
                    Name = "Grand Palldium",
                    Address = "Nassua",
                    CountryId = 2,
                    Rating = 4

                }



                );
        }
    }
}
