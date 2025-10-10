using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Backend.Dtos;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShelterServicesController : ControllerBase
    {
        private readonly NeonTechDbContext _context;

        public ShelterServicesController(NeonTechDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShelterService>>> GetAllShelterServices(int shelterId, int? serviceId)
        {
            var shelterServices = await _context.ShelterServices.ToListAsync();
            return Ok(shelterServices);
        }

        [HttpGet("{shelterId:int}/{serviceId:int?}")]
        public async Task<ActionResult<IEnumerable<ShelterService>>> GetShelterServices(int shelterId, int? serviceId)
        {
            var query = _context.ShelterServices.AsQueryable();

            query = query.Where(ss => ss.ShelterId == shelterId);

            if (serviceId.HasValue)
                query = query.Where(ss => ss.ServiceId == serviceId.Value);

            var result = await query.ToListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ShelterService>> CreateShelterService([FromBody] ShelterService dto)
        {
            bool exists = await _context.ShelterServices
                .AnyAsync(ss => ss.ShelterId == dto.ShelterId && ss.ServiceId == dto.ServiceId);

            if (exists) return BadRequest("Service already assigned to this shelter.");

            _context.ShelterServices.Add(dto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShelterServices), new { shelterId = dto.ShelterId, serviceId = dto.ServiceId }, dto);
        }

        [HttpPatch("{shelterId:int}/{serviceId:int}/price")]
        public async Task<IActionResult> UpdatePrice(int shelterId, int serviceId, [FromBody] decimal price)
        {
            var ss = await _context.ShelterServices.FindAsync(shelterId, serviceId);
            if (ss == null) return NotFound();

            ss.Price = price;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{shelterId:int}/{serviceId:int}/availability")]
        public async Task<IActionResult> UpdateAvailability(int shelterId, int serviceId, [FromBody] bool isAvailable)
        {
            var ss = await _context.ShelterServices.FindAsync(shelterId, serviceId);
            if (ss == null) return NotFound();

            ss.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{shelterId:int}/{serviceId:int}/description")]
        public async Task<IActionResult> UpdateDescriptione(int shelterId, int serviceId, [FromBody] string description)
        {
            var ss = await _context.ShelterServices.FindAsync(shelterId, serviceId);
            if (ss == null) return NotFound();

            ss.Description = description;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{shelterId:int}/{serviceId:int}/capacity")]
        public async Task<IActionResult> UpdatePrice(int shelterId, int serviceId, [FromBody] int capacity)
        {
            var ss = await _context.ShelterServices.FindAsync(shelterId, serviceId);
            if (ss == null) return NotFound();

            ss.Capacity = capacity;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{shelterId:int}/{serviceId:int}")]
        public async Task<IActionResult> DeleteShelterService(int shelterId, int serviceId)
        {
            var ss = await _context.ShelterServices.FindAsync(shelterId, serviceId);
            if (ss == null) return NotFound();

            _context.ShelterServices.Remove(ss);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}