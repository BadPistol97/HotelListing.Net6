﻿using Hotel.API.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotel.API.Repositories
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;

        public CountriesRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<Country> GetDetails(int id)
        {
            return await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
