using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SheltersController : ControllerBase
    {
        private readonly NeonTechDbContext _context;

        public SheltersController(NeonTechDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shelter>>> GetShelters()
        {
            var shelters = await _context.Shelters.ToListAsync();
            return Ok(shelters);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Shelter>> GetShelter(int id)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null) return NotFound();
            return Ok(shelter);
        }

        [HttpPost()]
        public async Task<ActionResult<Shelter>> CreateShelter([FromBody] ShelterCreateDto dto)
        {
            var shelter = new Shelter
            {
                Name = dto.Name,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Phone = dto.Phone,
                Capacity = dto.Capacity,
                Description = dto.Description,
                Occupancy = dto.Occupancy
            };

            _context.Shelters.Add(shelter);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetShelter), new { id = shelter.Id }, shelter);
        }



        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateShelter(int id, Shelter shelter)
        {
            if (id != shelter.Id) return BadRequest();

            var existing = await _context.Shelters.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = shelter.Name;
            existing.Address = shelter.Address;
            existing.Latitude = shelter.Latitude;
            existing.Longitude = shelter.Longitude;
            existing.Phone = shelter.Phone;
            existing.Capacity = shelter.Capacity;
            existing.Description = shelter.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/name")]
        public async Task<IActionResult> UpdateShelterName(int id, [FromBody] string name)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null) return NotFound();

            shelter.Name = name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/address")]
        public async Task<IActionResult> UpdateShelterAddress(int id, [FromBody] string address)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null) return NotFound();

            shelter.Address = address;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/coordinates")]
        public async Task<IActionResult> UpdateShelterCoordinates(int id, [FromBody] CoordinatesDto coords)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null) return NotFound();

            shelter.Latitude = coords.Latitude;
            shelter.Longitude = coords.Longitude;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/description")]
        public async Task<IActionResult> UpdateShelterDescription(int id, [FromBody] string description)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null) return NotFound();

            shelter.Description = description;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteShelter(int id)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null) return NotFound();

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class ShelterCreateDto
        {
            public string Name { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string Phone { get; set; } = string.Empty;
            public int Capacity { get; set; }
            public string? Description { get; set; }
            public int Occupancy { get; set; }
        }

        public class CoordinatesDto
        {
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
        }
    }
}