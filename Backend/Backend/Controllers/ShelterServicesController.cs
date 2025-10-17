using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ShelterServicesController : ControllerBase
    {
        private readonly IShelterServices _shelterServices;

        public ShelterServicesController(IShelterServices shelterServices)
        {
            _shelterServices = shelterServices;
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShelterService>>> GetShelterServices()
        {
            var response = await _shelterServices.GetShelterServices();
            return MapResponse(response);
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet("{shelterId:int}")]
        public async Task<ActionResult<IEnumerable<ShelterService>>> GetShelterServicesByShelter(int shelterId)
        {
            var response = await _shelterServices.GetShelterServicesByShelter(shelterId);
            return MapResponse(response);
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet("{shelterId:int}/{serviceId:int}")]
        public async Task<ActionResult<ShelterService>> GetShelterService(int shelterId, int serviceId)
        {
            var response = await _shelterServices.GetShelterService(shelterId, serviceId);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<ShelterService>> CreateShelterService([FromBody] ShelterServiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _shelterServices.CreateShelterService(dto);
            return MapResponse(response, created: true);
        }

        [HttpPut]
        public async Task<ActionResult<ShelterService>> UpdateShelterService([FromBody] ShelterServicePutDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _shelterServices.UpdateShelterService(dto);
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