using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.Countries;
using HotelListing.API.Models.Hotels;

namespace HotelListing.API.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();

            CreateMap<Inn, HotelDto>().ReverseMap();
            CreateMap<Inn, CreateHotelDto>().ReverseMap();
        }
    }
}
