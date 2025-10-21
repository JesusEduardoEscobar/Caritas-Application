using Backend.Dtos;
using Backend.Implementations;
using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infraestructure.Objects;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservations _reservations;

        public ReservationsController(IReservations reservations)
        {
            _reservations = reservations;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations([FromQuery] int? shelterId = null, [FromQuery] int? userId = null, [FromQuery] ReservationStatus? status = null, [FromQuery] DateTime? date = null)
        {
            var response = await _reservations.GetReservations(shelterId, userId, status, date);
            return MapResponse(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var response = await _reservations.GetReservation(id);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation([FromBody] ReservationCreateDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _reservations.CreateReservation(reservationDto);
            return MapResponse(response, created: true);
        }

        [HttpPatch("status")]
        public async Task<ActionResult<Reservation>> UpdateReservationStatus([FromBody] ReservationPatchStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _reservations.UpdateReservationStatus(dto);
            return MapResponse(response);
        }

        [HttpPatch("period")]
        public async Task<ActionResult<Reservation>> UpdateReservationPeriod([FromBody] ReservationPatchPeriodDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _reservations.UpdateReservationPeriod(dto);
            return MapResponse(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Reservation>> DeleteReservation(int id)
        {
            var response = await _reservations.DeleteReservation(id);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
            return response.Code switch
            {
                "200" => Ok(response),
                "201" => created ? CreatedAtAction(nameof(GetReservation), new { id = (response.Data as Reservation)?.Id }, response) : Ok(response),
                "400" => BadRequest(response),
                "404" => NotFound(response),
                _ => StatusCode(500, response)
            };
        }
    }
}