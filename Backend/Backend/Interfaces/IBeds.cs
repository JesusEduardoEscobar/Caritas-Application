using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Interfaces
{
    public interface IBeds
    {
        // Get
        Task<GlobalResponse<IEnumerable<Bed>>> GetBeds(int? shelterId = null, bool? available = null);
        Task<GlobalResponse<Bed>> GetBed(int id);

        // Post
        Task<GlobalResponse<Bed>> CreateBed(BedCreateDto bedDto);

        // Put
        Task<GlobalResponse<Bed>> UpdateBed(BedUpdateDto bedDto);

        // Patch
        Task<GlobalResponse<Bed>> UpdateBedAvailability(BedUpdateAvailabilityDto bedDto);

        // Delete
        Task<GlobalResponse<Bed>> DeleteBed(int id);
    }
}
