using Backend.Dtos;
using Backend.Implementations;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace Backend.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SheltersController : ControllerBase
    {
        private readonly IShelters _shelters;

        public SheltersController(IShelters shelters)
        {
            _shelters = shelters;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shelter>>> GetShelters()
        {
            var response = await _shelters.GetShelters();
            return MapResponse(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Shelter>> GetShelter(int id)
        {
            var response = await _shelters.GetShelter(id);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<Shelter>> CreateShelter([FromBody] ShelterCreateDto shelterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _shelters.CreateShelter(shelterDto);
            return MapResponse(response, created: true);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Shelter>> UpdateShelter(int id, [FromBody] ShelterUpdateDto shelterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            if (id != shelterDto.Id)
                return BadRequest(GlobalResponse<string>.Fault("El ID del cuerpo no coincide con la URL", "400", null));

            var response = await _shelters.UpdateShelter(shelterDto);
            return MapResponse(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Shelter>> DeleteShelter(int id)
        {
            var response = await _shelters.DeleteShelter(id);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
            return response.Code switch
            {
                "200" => Ok(response),
                "201" => created ? CreatedAtAction(nameof(GetShelter), new { id = (response.Data as Shelter)?.Id }, response) : Ok(response),
                "400" => BadRequest(response),
                "404" => NotFound(response),
                _ => StatusCode(500, response)
            };
        }
    }
}