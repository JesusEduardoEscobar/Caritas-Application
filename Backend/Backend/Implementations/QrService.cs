using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Backend.Implementations
{
    public class QrService : IQrService
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<QrService> _logger;

        public QrService(NeonTechDbContext context, ILogger<QrService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GlobalResponse<dynamic>> ReadQr(string qr)
        {
            try
            {
                string pattern = @"^(?<Type>Reservation|ServiceReservation|TransportRequest)-(?<Id>\d+)-User-(?<UserId>\d+)$";

                Match match = Regex.Match(qr, pattern);
                if (!match.Success)
                {
                    _logger.LogWarning("Qr {qr} con formato invalido.", qr);
                    return GlobalResponse<dynamic>.Fault("Qr con formato invalido", "400", null);
                }

                string type = match.Groups["Type"].Value;
                int id = int.Parse(match.Groups["Id"].Value);
                int userId = int.Parse(match.Groups["UserId"].Value);


                if (type == "Reservation")
                {
                    var entry = await _context.Reservations.FindAsync(id);
                    if(entry == null)
                    {
                        _logger.LogWarning("Reservacion {Id} no encontrada.", id);
                        return GlobalResponse<dynamic>.Fault("Reservacion no encontrada", "404", null);
                    }

                    var userEntry = await _context.Users.FindAsync(entry.UserId);
                    if (userEntry == null || userId != userEntry.Id)
                    {
                        _logger.LogWarning("Usuario {Id} no encontrado.", entry.UserId);
                        return GlobalResponse<dynamic>.Fault("Usuario no encontrada", "404", null);
                    }

                    _logger.LogInformation("Reservacion con QR {qr} obtenida correctamente.", qr);
                    return GlobalResponse<dynamic>.Success(
                        new QrReadResponse { Type = entry.GetType().Name, Id = entry.Id, UserId = userEntry.Id },
                        1, "Obtención de Reservacion exitosa", "200"
                    );
                }
                else if (type == "ServiceReservation")
                {
                    var entry = await _context.ServiceReservations.FindAsync(id);
                    if (entry == null)
                    {
                        _logger.LogWarning("Reservacion de servicio {Id} no encontrada.", id);
                        return GlobalResponse<dynamic>.Fault("Reservacion de servicio no encontrada", "404", null);
                    }

                    var userEntry = await _context.Users.FindAsync(entry.UserId);
                    if (userEntry == null || userId != userEntry.Id)
                    {
                        _logger.LogWarning("Usuario {Id} no encontrado.", entry.UserId);
                        return GlobalResponse<dynamic>.Fault("Usuario no encontrada", "404", null);
                    }

                    _logger.LogInformation("Reservacion con QR {qr} obtenida correctamente.", qr);
                    return GlobalResponse<dynamic>.Success(
                        new QrReadResponse { Type = entry.GetType().Name, Id = entry.Id, UserId = userEntry.Id },
                        1, "Obtención de Reservacion exitosa", "200"
                    );
                }
                else if (type == "TransportRequest")
                {
                    var entry = await _context.TransportRequests.FindAsync(id);
                    if (entry == null)
                    {
                        _logger.LogWarning("Peticion de Transporte {Id} no encontrada.", id);
                        return GlobalResponse<dynamic>.Fault("Peticion de Transporte no encontrada", "404", null);
                    }

                    var userEntry = await _context.Users.FindAsync(entry.UserId);
                    if (userEntry == null || userId != userEntry.Id)
                    {
                        _logger.LogWarning("Usuario {Id} no encontrado.", entry.UserId);
                        return GlobalResponse<dynamic>.Fault("Usuario no encontrada", "404", null);
                    }

                    _logger.LogInformation("Reservacion con QR {qr} obtenida correctamente.", qr);
                    return GlobalResponse<dynamic>.Success(
                        new QrReadResponse { Type = entry.GetType().Name, Id = entry.Id, UserId = userEntry.Id },
                        1, "Obtención de Reservacion exitosa", "200"
                    );
                }

                _logger.LogWarning("Tipo {type} no encontrado.", type);
                return GlobalResponse<dynamic>.Fault("Tipo de reservacion no encontrada", "400", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer qr.");
                return GlobalResponse<dynamic>.Fault("Error al leer Qr", "-1", null);
            }
        }

        public async Task<GlobalResponse<string>> GenerateReservationQr(int id)
        {
            try
            {
                var entry = await _context.Reservations.FindAsync(id);
                if (entry == null)
                {
                    _logger.LogWarning("Reservacion {id} no encontrada.", id);
                    return GlobalResponse<string>.Fault("Reservacion no encontrada", "404", null);
                }

                var userExists = await _context.Users.AnyAsync(u => u.Id == entry.UserId);
                if (!userExists)
                {
                    _logger.LogWarning("Usuario {Id} no encontrado.", entry.UserId);
                    return GlobalResponse<string>.Fault("Usuario no encontrada", "404", null);
                }

                string qr = $"Reservation-{entry.Id}-User-{entry.UserId}";

                _logger.LogInformation("Qr de Reservacion {Id} generado correctamente.", entry.Id);
                return GlobalResponse<string>.Success(qr, 1, "Generacion de Qr de Reservacion exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar qr de reservacion.");
                return GlobalResponse<string>.Fault("Error al generar Qr de Reservacion", "-1", null);
            }
        }

        public async Task<GlobalResponse<string>> GenerateServiceReservationQr(int id)
        {
            try
            {
                var entry = await _context.ServiceReservations.FindAsync(id);
                if (entry == null)
                {
                    _logger.LogWarning("Reservacion de Servicio {id} no encontrada.", id);
                    return GlobalResponse<string>.Fault("Reservacion de Servicio no encontrada", "404", null);
                }

                var userExists = await _context.Users.AnyAsync(u => u.Id == entry.UserId);
                if (!userExists)
                {
                    _logger.LogWarning("Usuario {Id} no encontrado.", entry.UserId);
                    return GlobalResponse<string>.Fault("Usuario no encontrada", "404", null);
                }

                string qr = $"ServiceReservation-{entry.Id}-User-{entry.UserId}";

                _logger.LogInformation("Qr de Reservacion de Servicio {Id} generado correctamente.", entry.Id);
                return GlobalResponse<string>.Success(qr, 1, "Generacion de Qr de Reservacion de Servicio exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar qr de reservacion de servicio.");
                return GlobalResponse<string>.Fault("Error al generar Qr de Reservacion de Servicio", "-1", null);
            }
        }

        public async Task<GlobalResponse<string>> GenerateTransportRequestQr(int id)
        {
            try
            {
                var entry = await _context.TransportRequests.FindAsync(id);
                if (entry == null)
                {
                    _logger.LogWarning("Peticion de Transporte {id} no encontrada.", id);
                    return GlobalResponse<string>.Fault("Peticion de Transporte no encontrada", "404", null);
                }

                var userExists = await _context.Users.AnyAsync(u => u.Id == entry.UserId);
                if (!userExists)
                {
                    _logger.LogWarning("Usuario {Id} no encontrado.", entry.UserId);
                    return GlobalResponse<string>.Fault("Usuario no encontrada", "404", null);
                }

                string qr = $"TransportRequest-{entry.Id}-User-{entry.UserId}";

                _logger.LogInformation("Qr de Peticion de Transporte {Id} generado correctamente.", entry.Id);
                return GlobalResponse<string>.Success(qr, 1, "Generacion de Qr de Peticion de Transporte exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar qr de peticion de transporte.");
                return GlobalResponse<string>.Fault("Error al generar Qr de peticion de transporte", "-1", null);
            }
        }

    }

    public class QrReadResponse
    {
        public string Type { get; set; } = string.Empty;
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}
