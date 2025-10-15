using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface ICars
    {
        // Get
        Task<GlobalResponse<IEnumerable<Car>>> GetCars(int? shelterId = null);
        Task<GlobalResponse<Car>> GetCar(int id);

        // Post
        Task<GlobalResponse<Car>> CreateCar(CarCreateDto dto);

        // Put
        Task<GlobalResponse<Car>> UpdateCar(CarPutDto dto);

        // Delete
        Task<GlobalResponse<Car>> DeleteCar(int id);
    }
}
