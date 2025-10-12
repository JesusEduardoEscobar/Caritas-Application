using Backend.Dtos;
using Backend.Implementations;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infraestructure.Objects;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BedsController : ControllerBase
    {
        private readonly IBeds _beds;

        public BedsController(IBeds beds)
        {
            _beds = beds;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bed>>> GetBeds([FromQuery] int? shelterId = null, [FromQuery] bool? available = null)
        {
            var response = await _beds.GetBeds(shelterId, available);
            return MapResponse(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Bed>> GetBed(int shelterId)
        {
            var response = await _beds.GetBed(shelterId);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<Bed>> CreateBed([FromBody] BedCreateDto bedDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _beds.CreateBed(bedDto);
            return MapResponse(response, created: true);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Bed>> UpdateBed(int id, [FromBody] BedUpdateDto bedDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            if (id != bedDto.Id)
                return BadRequest(GlobalResponse<string>.Fault("El ID del cuerpo no coincide con la URL", "400", null));

            var response = await _beds.UpdateBed(bedDto);
            return MapResponse(response);
        }

        [HttpPatch("{id:int}/availability")]
        public async Task<ActionResult<Bed>> UpdateBedAvailability(int id, [FromBody] BedUpdateAvailabilityDto bedDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            if (id != bedDto.Id)
                return BadRequest(GlobalResponse<string>.Fault("El ID del cuerpo no coincide con la URL", "400", null));

            var response = await _beds.UpdateBedAvailability(bedDto);
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