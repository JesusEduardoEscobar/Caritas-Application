using Backend.Infrastructure.Dtos;
using Backend.Infraestructure.Interfaces;
using QRCoder;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.IO;
using Backend.Infraestructure.Dtos;

[ApiController]
[Route("api/[controller]")]
public class BedQrController : ControllerBase
{
    private readonly IBedReservations _bedReservations;

    public BedQrController(IBedReservations bedReservations)
    {
        _bedReservations = bedReservations;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateQr([FromBody] BedQrRequest request)
    {
        if (request == null) return BadRequest("❌ Datos inválidos");

        try
        {
            // convertir fechas a UTC
            var startUtc = request.StartDate.Kind == DateTimeKind.Utc ? request.StartDate : request.StartDate.ToUniversalTime();
            var endUtc = request.EndDate.Kind == DateTimeKind.Utc ? request.EndDate : request.EndDate.ToUniversalTime();

            // generar qr text (podés cambiar formato)
            var qrText = $"USER-{request.UserId}-BED-{request.BedId}-{Guid.NewGuid()}";

            // crear DTO y asignar enum
            var dto = new BedReservationCreateDto
            {
                UserId = request.UserId,
                BedId = request.BedId,
                StartDate = startUtc,
                EndDate = endUtc,
                Status = ReservationStatus.reserved, // enum aquí
                CreatedAt = DateTime.UtcNow,
                QrData = qrText
            };

            var saveResult = await _bedReservations.CreateReservation(dto);
            if (!saveResult.Ok) return StatusCode(500, saveResult.Message);

            // generar imagen QR usando qrText (no usar saveResult.Data para el texto aquí)
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using var bitmap = qrCode.GetGraphic(20);

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            var base64Image = Convert.ToBase64String(ms.ToArray());

            return Ok(new { qrData = qrText, qrImage = $"data:image/png;base64,{base64Image}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"❌ Error al generar el QR: {ex.Message}");
        }
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateQr([FromBody] BedQrValidationRequest body)
    {
        if (string.IsNullOrEmpty(body?.QrText)) return BadRequest("❌ QR vacío");

        var validation = await _bedReservations.ValidateQr(body.QrText);
        if (!validation.Ok) return NotFound(validation.Message);
        return Ok(validation);
    }
}

public class BedQrRequest
{
    public int UserId { get; set; }
    public int BedId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
public class BedQrValidationRequest { public string QrText { get; set; } = string.Empty; }
