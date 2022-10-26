using Hotel.API.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotel.API.Repositories
{
    public class HotelsRepository : GenericRepository<HotelListing.API.Data.Inn>, IHotelsRepository
    {

        private readonly HotelListingDbContext _context;

        public HotelsRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }
    }
}
