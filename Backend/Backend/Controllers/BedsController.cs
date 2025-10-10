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
        private readonly NeonTechDbContext _context;
        private readonly IBeds _beds;

        public BedsController(NeonTechDbContext context, IBeds beds)
        {
            _context = context;
            _beds = beds;
        }

        #region GET

        [HttpGet]
        public async Task<IActionResult> GetBeds()
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableBeds()
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("shelter/{shelterId:int}")]
        public async Task<IActionResult> GetBedsByShelter(int shelterId)
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("shelter/{shelterId:int}/available")]
        public async Task<IActionResult> GetAvailableBedsByShelter(int shelterId)
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        #endregion

        #region POST

        [HttpPost]
        public async Task<IActionResult> CreateBed([FromBody] Bed bed)
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        #endregion

        #region PUT

        [HttpPut("{shelterId:int}")]
        public async Task<IActionResult> UpdateBed([FromBody] Bed bed)
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        #endregion

        #region PATCH

        [HttpPatch("{shelterId:int}/number")]
        public async Task<IActionResult> UpdateBedNumber(int id, [FromBody] int bedNumber)
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPatch("{shelterId:int}/availability")]
        public async Task<IActionResult> UpdateBedAvailability(int id, [FromBody] bool isAvailable)
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        #endregion

        #region DELETE

        [HttpDelete("{shelterId:int}")]
        public async Task<IActionResult> DeleteBed(int id)
        {
            try
            {
                var response = GlobalResponse<string>.Fault("Sin Implementación", "-1", null);
                return StatusCode(501, response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        #endregion

    }
}