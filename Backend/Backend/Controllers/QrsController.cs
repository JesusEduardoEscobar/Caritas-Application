using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using DocumentFormat.OpenXml.Office.Word.Y2020.OEmbed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QrsController : ControllerBase
    {
        private readonly IQrService _qrService;
        
        public QrsController(IQrService qrService)
        {
            _qrService = qrService;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("{qr}")]
        public async Task<ActionResult<dynamic>> ReadQr(string qr)
        {
            var response = await _qrService.ReadQr(qr);
            return MapResponse(response);
        }

        [HttpGet("reservation/{id:int}")]
        public async Task<ActionResult<string>> GenerateReservationQr(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _qrService.GenerateReservationQr(id);
            return MapResponse(response);
        }

        [HttpGet("service-reservation/{id:int}")]
        public async Task<ActionResult<string>> GenerateServiceReservationQr(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _qrService.GenerateServiceReservationQr(id);
            return MapResponse(response);
        }

        [HttpGet("transport-request/{id:int}")]
        public async Task<ActionResult<string>> GenerateTransportRequestQr(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _qrService.GenerateTransportRequestQr(id);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
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