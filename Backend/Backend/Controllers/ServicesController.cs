using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServices _services;

        public ServicesController(IServices services)
        {
            _services = services;
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var response = await _services.GetServices();
            return MapResponse(response);
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var response = await _services.GetService(id);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<Service>> CreateService([FromBody] ServiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _services.CreateService(dto);
            return MapResponse(response, created: true);
        }

        [HttpPut]
        public async Task<ActionResult<Service>> UpdateShelter([FromBody] ServicePutDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _services.UpdateService(dto);
            return MapResponse(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Service>> DeleteShelter(int id)
        {
            var response = await _services.DeleteService(id);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
            return response.Code switch
            {
                "200" => Ok(response),
                "201" => created ? CreatedAtAction(nameof(GetService), new { id = (response.Data as Service)?.Id }, response) : Ok(response),
                "400" => BadRequest(response),
                "404" => NotFound(response),
                _ => StatusCode(500, response)
            };
        }
    }
}