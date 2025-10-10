using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Backend.Controllers
{

    /// Controlador para la generación y validación de códigos QR.

    [ApiController]
    [Route("api/[controller]")]
    public class QrController : ControllerBase
    {

        /// Genera un código QR a partir de los datos del servicio o check-in.
        [HttpPost("generate")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult GenerateQr([FromBody] QrRequest request)
        {
            if (request == null)
                return BadRequest("❌ Datos inválidos");

            try
            {
                // Formato de la cadena QR
                string qrData = $"{request.Shelter}-{request.UserId}-{request.Service}-{request.Frequency}-{request.Persons}-{request.Time}";

                // Generar QR como imagen
                using var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(qrCodeData);
                using var bitmap = qrCode.GetGraphic(20);

                // Convertir imagen a Base64
                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                string base64Image = Convert.ToBase64String(ms.ToArray());

                return Ok(new
                {
                    qrData,
                    qrImage = $"data:image/png;base64,{base64Image}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error al generar el QR: {ex.Message}");
            }
        }

   
        /// Valida el contenido de un código QR escaneado.

        [HttpPost("validate")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult ValidateQr([FromBody] QrValidationRequest qrData)
        {
            if (string.IsNullOrEmpty(qrData?.QrText))
                return BadRequest("❌ QR vacío o inválido");

            try
            {
                var parts = qrData.QrText.Split('-');
                if (parts.Length != 6)
                    return BadRequest("❌ Formato de QR no válido");

                return Ok(new
                {
                    Shelter = parts[0],
                    UserId = parts[1],
                    Service = parts[2],
                    Frequency = parts[3],
                    Persons = parts[4],
                    Time = parts[5]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error al validar el QR: {ex.Message}");
            }
        }
    }


    /// Datos para generar el QR.
    public class QrRequest
    {
        public string Shelter { get; set; }
        public string UserId { get; set; }
        public string Service { get; set; }
        public string Frequency { get; set; }
        public int Persons { get; set; }
        public string Time { get; set; }
    }


    /// Datos para validar un QR existente.
    public class QrValidationRequest
    {
        public string QrText { get; set; }
    }
}
