using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShelterServicesController : ControllerBase
    {
        private readonly IShelterServices _shelterServices;

        public ShelterServicesController(IShelterServices shelterServices)
        {
            _shelterServices = shelterServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShelterService>>> GetShelterServices()
        {
            var response = await _shelterServices.GetShelterServices();
            return MapResponse(response);
        }

        [HttpGet("{shelterId:int}")]
        public async Task<ActionResult<IEnumerable<ShelterService>>> GetShelterServicesByShelter(int shelterId)
        {
            var response = await _shelterServices.GetShelterServicesByShelter(shelterId);
            return MapResponse(response);
        }

        [HttpGet("{shelterId:int}/{serviceId:int}")]
        public async Task<ActionResult<ShelterService>> GetShelterService(int shelterId, int serviceId)
        {
            var response = await _shelterServices.GetShelterService(shelterId, serviceId);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<ShelterService>> CreateShelterService([FromBody] ShelterServiceCreateDto shelterServiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _shelterServices.CreateShelterService(shelterServiceDto);
            return MapResponse(response, created: true);
        }

        [HttpPut("{shelterId:int}/{serviceId:int}")]
        public async Task<ActionResult<ShelterService>> UpdateShelterService(int shelterId, int serviceId, [FromBody] ShelterServiceUpdateDto shelterServiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            if (shelterId != shelterServiceDto.ShelterId || serviceId != shelterServiceDto.ServiceId)
                return BadRequest(GlobalResponse<string>.Fault("Los IDs del cuerpo no coincide con la URL", "400", null));

            var response = await _shelterServices.UpdateShelterService(shelterServiceDto);
            return MapResponse(response);
        }

        [HttpDelete("{shelterId:int}/{serviceId:int}")]
        public async Task<ActionResult<ShelterService>> DeleteShelterService(int shelterId, int serviceId)
        {
            var response = await _shelterServices.DeleteShelterService(shelterId, serviceId);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
            if (response.Code == "201" && created)
            {
                if (response.Data is ShelterService ss)
                    return CreatedAtAction("GetShelterService", new { shelterId = ss.ShelterId, serviceId = ss.ServiceId }, response);

                return Ok(response);
            }
            return response.Code switch
            {
                "200" => Ok(response),
                "400" => BadRequest(response),
                "404" => NotFound(response),
                _ => StatusCode(500, response)
            };
        }
    }
}