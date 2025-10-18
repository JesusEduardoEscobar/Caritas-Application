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
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceReservationsController : ControllerBase
    {
        private readonly IServiceReservations _serviceReservations;

        public ServiceReservationsController(IServiceReservations serviceReservations)
        {
            _serviceReservations = serviceReservations;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceReservation>>> GetServiceReservations([FromQuery] int? userId = null, [FromQuery] int? shelterId = null, [FromQuery] int? serviceId = null, [FromQuery] bool? isActive = null)
        {
            var response = await _serviceReservations.GetServiceReservations(userId, shelterId, serviceId, isActive);
            return MapResponse(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceReservation>> GetServiceReservation(int id)
        {
            var response = await _serviceReservations.GetServiceReservation(id);
            return MapResponse(response);
        }

        [HttpGet("qr/{qrData}")]
        public async Task<ActionResult<ServiceReservation>> GetServiceReservation(string qrData)
        {
            var response = await _serviceReservations.GetServiceReservation(qrData);
            return MapResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceReservation>> CreateServiceReservation([FromBody] ServiceReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _serviceReservations.CreateServiceReservation(dto);
            return MapResponse(response, created: true);
        }

        [HttpPatch("is-active")]
        public async Task<ActionResult<ServiceReservation>> UpdateServiceReservationIsActive([FromBody] ServiceReservationPatchIsActiveDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(GlobalResponse<string>.Fault("Datos inválidos", "400", null));

            var response = await _serviceReservations.UpdateServiceReservationIsActive(dto);
            return MapResponse(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ServiceReservation>> DeleteServiceReservation(int id)
        {
            var response = await _serviceReservations.DeleteServiceReservation(id);
            return MapResponse(response);
        }



        private ActionResult<T> MapResponse<T>(GlobalResponse<T> response, bool created = false) where T : class
        {
            return response.Code switch
            {
                "200" => Ok(response),
                "201" => created ? CreatedAtAction(nameof(GetServiceReservation), new { id = (response.Data as ServiceReservation)?.Id }, response) : Ok(response),
                "400" => BadRequest(response),
                "404" => NotFound(response),
                _ => StatusCode(500, response)
            };
        }

        //[HttpPost("generate")]
        //public async Task<IActionResult> GenerateQr([FromBody] QrRequest request)
        //{
        //    if (request == null) return BadRequest("❌ Datos inválidos");

        //    try
        //    {
        //        // Formato legible del QR
        //        string qrData = $"{request.Shelter}-{request.UserId}-{request.Service}-{request.Frequency}-{request.Persons}-{request.Time}";

        //        // Generar imagen QR
        //        using var qrGenerator = new QRCodeGenerator();
        //        var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
        //        using var qrCode = new QRCode(qrCodeData);
        //        using var bitmap = qrCode.GetGraphic(20);

        //        using var ms = new MemoryStream();
        //        bitmap.Save(ms, ImageFormat.Png);
        //        string base64Image = Convert.ToBase64String(ms.ToArray());

        //        // Guardar en BD (DTO)
        //        var createDto = new ServiceReservationCreateDto
        //        {
        //            UserId = request.UserId,
        //            ShelterId = request.ShelterId,
        //            ServiceId = request.ServiceId,
        //            QrData = qrData
        //        };

        //        var saveResult = await _serviceReservations.CreateReservation(createDto);

        //        // Si tu GlobalResponse tiene Data / Code, comprueba aquí (ej: saveResult == null o Data == null)
        //        // Asumimos que en caso de fallo CreateReservation devuelve Fault -> Data será null
        //        // Ajusta según tu GlobalResponse real
        //        if (!saveResult.Ok)
        //        {
        //            return StatusCode(500, $"❌ Error al guardar la reserva: {saveResult.Message}");
        //        }

        //        return Ok(new
        //        {
        //            qrData,
        //            qrImage = $"data:image/png;base64,{base64Image}"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"❌ Error al generar el QR: {ex.Message}");
        //    }
        //}

        //[HttpPost("validate")]
        //public async Task<IActionResult> ValidateQr([FromBody] QrValidationRequest qrData)
        //{
        //    if (string.IsNullOrEmpty(qrData?.QrText))
        //        return BadRequest("❌ QR vacío o inválido");

        //    try
        //    {
        //        var parts = qrData.QrText.Split('-');
        //        if (parts.Length != 6)
        //            return BadRequest("❌ Formato de QR no válido");

        //        // Validar contra BD
        //        var validationResult = await _serviceReservations.ValidateQr(qrData.QrText);
        //        if (!validationResult.Ok)
        //        {
        //            return StatusCode(500, $"❌ Error en validación: {validationResult.Message}");
        //            }

        //        // Si tu GlobalResponse devuelve Fault con Data null -> interpretarlo
        //        // Aquí devolvemos lo que haya venido del servicio (ajusta según GlobalResponse)
        //        return Ok(validationResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"❌ Error al validar el QR: {ex.Message}");
        //    }
        //}
    }

    //// Request bodies (ajusta tipos para usar ints para ids)
    //public class QrRequest
    //{
    //    public int UserId { get; set; }
    //    public int ShelterId { get; set; }
    //    public int ServiceId { get; set; }

    //    // nombres para componer el string del QR (opcional)
    //    public string Shelter { get; set; } = string.Empty;
    //    public string Service { get; set; } = string.Empty;
    //    public string Frequency { get; set; } = string.Empty;
    //    public int Persons { get; set; }
    //    public string Time { get; set; } = string.Empty;
    //}

    //public class QrValidationRequest
    //{
    //    public string QrText { get; set; } = string.Empty;
    //}
}
