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
    public class ServicesController : ControllerBase
    {
        private readonly NeonTechDbContext _context;

        public ServicesController(NeonTechDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _context.Services.ToListAsync();
            return Ok(services);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();
            return Ok(service);
        }

        [HttpPost()]
        public async Task<ActionResult<Service>> CreateService([FromBody] ServiceCreateDto dto)
        {
            var service = new Service
            {
                Name = dto.Name,
                Description = dto.Description,
                IconKey = dto.IconKey
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }



        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> UpdateShelter(int id, Shelter shelter)
        //{
        //    if (id != shelter.Id) return BadRequest();

        //    var existing = await _context.Shelters.FindAsync(id);
        //    if (existing == null) return NotFound();

        //    existing.Name = shelter.Name;
        //    existing.Address = shelter.Address;
        //    existing.Latitude = shelter.Latitude;
        //    existing.Longitude = shelter.Longitude;
        //    existing.Phone = shelter.Phone;
        //    existing.Capacity = shelter.Capacity;
        //    existing.Description = shelter.Description;

        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpPatch("{id:int}/name")]
        public async Task<IActionResult> UpdateServiceName(int id, [FromBody] string name)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.Name = name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/description")]
        public async Task<IActionResult> UpdateServiceDescription(int id, [FromBody] string description)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.Description = description;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/icon")]
        public async Task<IActionResult> UpdateShelterCoordinates(int id, [FromBody] string iconKey)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.IconKey = iconKey;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteShelter(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}