using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Countries;
using AutoMapper;
using Hotel.API.Contracts;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly ICountriesRepository _countriesRepository; 
        private readonly IMapper _mapper;

        public CountriesController(ICountriesRepository countriesRepository, IMapper mapper)
        {
            _countriesRepository = countriesRepository;
            _mapper = mapper;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {

            IEnumerable<Country> countries = await _countriesRepository.GetAllAsync();

            return Ok(_mapper.Map<IEnumerable<GetCountryDto>>(countries));

        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {

            //Country country = await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(q => q.Id == id);

            Country country = await _countriesRepository.GetDetails(id);

            if (country == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CountryDto>(country));
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest("Invalid id");
            }

            //_context.Entry(country).State = EntityState.Modified;

            //Country country = await _context.Countries.FindAsync(id);

            Country country = await _countriesRepository.GetAsync(id);

            if(country == null)
            {
                return NotFound();
            }

            _mapper.Map(updateCountryDto,country);

            try
            {
                //await _context.SaveChangesAsync();

                await _countriesRepository.UpdateAsync(country);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {

            Country country = _mapper.Map<Country>(createCountryDto);

            //_context.Countries.Add(country);

            //await _context.SaveChangesAsync();

            await _countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            //var country = await _context.Countries.FindAsync(id);

            Country country = await _countriesRepository.GetAsync(id);
            
            if (country == null)
            {
                return NotFound();
            }

            //_context.Countries.Remove(country);
            //await _context.SaveChangesAsync();

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            //return _context.Countries.Any(e => e.Id == id);

            return await _countriesRepository.ExistsAsync(id);
        }
    }
}
