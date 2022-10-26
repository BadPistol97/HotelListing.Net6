using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using Hotel.API.Contracts;
using Hotel.API.Repositories;
using HotelListing.API.Models.Hotels;
using System.Diagnostics.Metrics;
using AutoMapper;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : Controller
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly IMapper _mapper;

        public HotelsController(IHotelsRepository hotelsRepository, IMapper mapper)
        {
            this._hotelsRepository = hotelsRepository;
            this._mapper = mapper;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            List<Inn> hotels = await _hotelsRepository.GetAllAsync();

            return Ok(_mapper.Map<List<HotelDto>>(hotels));

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            Inn hotel = await _hotelsRepository.GetAsync(id);

            if(hotel == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Inn>(hotel));
        }

        [HttpPut]
        public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
        {
            if(id != hotelDto.Id)
            {
                return BadRequest();
            }

            Inn hotel = await _hotelsRepository.GetAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            _mapper.Map(hotelDto, hotel);

            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
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


        [HttpPost]
        public async Task<ActionResult<Inn>> PostHotel(CreateHotelDto hotelDto)
        {
            Inn hotel = _mapper.Map<Inn>(hotelDto);

            await _hotelsRepository.AddAsync(hotel);

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            Inn hotel = await _hotelsRepository.GetAsync(id);

            if(hotel != null)
            {
                await _hotelsRepository.DeleteAsync(id);
            }

            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.ExistsAsync(id);
        }
    }
}
