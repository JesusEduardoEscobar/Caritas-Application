using Backend.Interfaces;
using Backend.Dtos;
using Backend.Infraestructure.Models;
using QRCoder;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Backend.Infraestructure.Implementations;

namespace Backend.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TransportRequestsController : ControllerBase
    {
        private readonly ITransportRequests _transportRequests;

        public TransportRequestsController(ITransportRequests transportRequests)
        {
            _transportRequests = transportRequests;
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransportRequest>>> GetTransportRequests([FromQuery] int? userId = null, [FromQuery] int? shelterId = null, [FromQuery] DateTime? requestDate = null, [FromQuery] ReservationStatus? status = null)
        {
            var response = await _transportRequests.GetTransportRequests(userId, shelterId, requestDate, status);
            return MapResponse(response);
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TransportRequest>> GetTransportRequest(int id)
        {
            var response = await _transportRequests.GetTransportRequest(id);
            return MapResponse(response);
        }

        //[HttpGet("qr/{qrData:string}")]
        //public async Task<ActionResult<TransportRequest>> GetTransportRequest(string qrData)
        //{
        //    var response = await _transportRequests.GetTransportRequest(qrData);
        //    return MapResponse(response);
        //}

        [HttpPost]
        public async Task<ActionResult<TransportRequest>> CreateTransportRequest([FromBody] TransportRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _transportRequests.CreateTransportRequest(dto);
            return MapResponse(response, created: true);
        }

        [HttpPatch]
        public async Task<ActionResult<TransportRequest>> UpdateTransportRequest([FromBody] TransportRequestPatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _transportRequests.UpdateTransportRequest(dto);
            return MapResponse(response);
        }

        [HttpPatch("status")]
        public async Task<ActionResult<TransportRequest>> UpdateTransportRequestStatus([FromBody] TransportRequestPatchStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _transportRequests.UpdateTransportRequestStatus(dto);
            return MapResponse(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<TransportRequest>> DeleteTransportRequest(int id)
        {
            var response = await _transportRequests.DeleteTransportRequest(id);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
            return response.Code switch
            {
                "200" => Ok(response),
                "201" => created ? CreatedAtAction(nameof(GetTransportRequest), new { id = (response.Data as TransportRequest)?.Id }, response) : Ok(response),
                "400" => BadRequest(response),
                "404" => NotFound(response),
                _ => StatusCode(500, response)
            };
        }

    }
}
