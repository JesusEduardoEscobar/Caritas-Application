using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class BedsController : ControllerBase
    {
        private readonly IBeds _beds;

        public BedsController(IBeds beds)
        {
            _beds = beds;
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet("GetAllBeds")]
        public async Task<ActionResult<IEnumerable<Bed>>> GetAllBeds()
        {
            var response = await _beds.GetBeds();
            return MapResponse(response);
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bed>>> GetBeds([FromQuery] int? shelterId = null, [FromQuery] bool? available = null)
        {
            var response = await _beds.GetBeds(shelterId, available);
            return MapResponse(response);
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Bed>> GetBed(int id)
        {
            var response = await _beds.GetBed(id);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<Bed>> CreateBed([FromBody] BedCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _beds.CreateBed(dto);
            return MapResponse(response, created: true);
        }

        [HttpPut]
        public async Task<ActionResult<Bed>> UpdateBed([FromBody] BedPutDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _beds.UpdateBed(dto);
            return MapResponse(response);
        }

        [HttpPatch("availability")]
        public async Task<ActionResult<Bed>> UpdateBedAvailability([FromBody] BedPatchAvailabilityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _beds.UpdateBedAvailability(dto);
            return MapResponse(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Bed>> DeleteBed(int id)
        {
            var response = await _beds.DeleteBed(id);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
            return response.Code switch
            {
                "200" => Ok(response),
                "201" => created ? CreatedAtAction(nameof(GetBed), new { id = (response.Data as Bed)?.Id }, response) : Ok(response),
                "400" => BadRequest(response),
                "404" => NotFound(response),
                _ => StatusCode(500, response)
            };
        }
    }
}