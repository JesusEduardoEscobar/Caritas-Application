using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Interfaces
{
    public interface IShelters
    {
        // Get
        Task<GlobalResponse<IEnumerable<Shelter>>> GetShelters();
        Task<GlobalResponse<Shelter>> GetShelter(int id);

        // Post
        Task<GlobalResponse<Shelter>> CreateShelter(ShelterCreateDto shelter);

        // Put
        Task<GlobalResponse<Shelter>> UpdateShelter(ShelterUpdateDto shelter);

        // Delete
        Task<GlobalResponse<Shelter>> DeleteShelter(int id);
    }
}
