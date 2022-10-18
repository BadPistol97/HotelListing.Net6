using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Country
{
    public abstract class BaseCountryDto
    {
        [Required]
        public String Name { get; set; }
        public String ShortName { get; set; }

    }
}
