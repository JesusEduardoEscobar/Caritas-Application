using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;

namespace Backend.Interfaces
{
    public interface IQrService
    {
        Task<GlobalResponse<dynamic>> ReadQr(string qr);

        Task<GlobalResponse<string>> GenerateReservationQr(int id);
        Task<GlobalResponse<string>> GenerateServiceReservationQr(int id);
        Task<GlobalResponse<string>> GenerateTransportRequestQr(int id);
    }
}
