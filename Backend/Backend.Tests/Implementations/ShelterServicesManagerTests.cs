using Backend.Dtos;
using Backend.Implementations;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using Backend.Tests.TestHelpers;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class ShelterServicesManagerTests
    {
        private ShelterServicesManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<ShelterServicesManager>();
            return new ShelterServicesManager(context, logger);
        }

        private ShelterService CreateDefaultShelterService(int? shelterId = null, int? serviceId = null)
        {
            var shelterService = new ShelterService
            {
                Price = 10,
                IsAvailable = true,
                Description = "Test Description",
                Capacity = 10
            };

            if (shelterId.HasValue)
                shelterService.ShelterId = shelterId.Value;
            if (serviceId.HasValue)
                shelterService.ServiceId = serviceId.Value;

            return shelterService;
        }

        #region GET

        [Fact(DisplayName = "GetShelterServices - Retorna 404 cuando no hay ShelterServices, y Data es Null")]
        public async Task GetShelterServices_ReturnsEmpty_WhenNoShelterServices()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetShelterServices();

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelterServices - Retorna la lista correctamente cuando hay 1 ShelterService o más")]
        public async Task GetShelterServices_ReturnsShelterServices_WhenExist()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelterService1 = CreateDefaultShelterService(shelterId: 1, serviceId: 1);
            var shelterService2 = CreateDefaultShelterService(shelterId: 1, serviceId: 2);
            context.ShelterServices.Add(shelterService1);
            context.ShelterServices.Add(shelterService2);
            await context.SaveChangesAsync();

            var response = await manager.GetShelterServices();

            Assert.Equal("200", response.Code);
            Assert.Equal(2, response.RowsCount);
            Assert.NotNull(response.Data);
            Assert.Equal(shelterService1.ShelterId, response.Data.First().ShelterId);
            Assert.Equal(shelterService1.Description, response.Data.First().Description);
        }

        [Fact(DisplayName = "GetShelterServicesByShelter - Retorna 404 cuando no hay ShelterServices, y Data es Null")]
        public async Task GetShelterServicesByShelter_ReturnsEmpty_WhenNoShelterServicesWithShelterId()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetShelterServicesByShelter(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelterServicesByShelter - Retorna la lista correctamente cuando hay 1 ShelterService o más")]
        public async Task GetShelterServicesByShelter_ReturnsShelterServices_WhenExistWithShelterId()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = new Shelter
            {
                Id = 5 // custom id 5
            };
            var service1 = new Service();
            var service2 = new Service();
            context.Shelters.Add(shelter);
            context.Services.Add(service1);
            context.Services.Add(service2);
            await context.SaveChangesAsync();

            var shelterService1 = CreateDefaultShelterService(shelter.Id, service1.Id);
            var shelterService2 = CreateDefaultShelterService(shelter.Id, service2.Id);
            context.ShelterServices.Add(shelterService1);
            context.ShelterServices.Add(shelterService2);
            await context.SaveChangesAsync();

            var response = await manager.GetShelterServicesByShelter(5); // custom id 5

            Assert.Equal("200", response.Code);
            Assert.Equal(2, response.RowsCount);
            Assert.NotNull(response.Data);
            Assert.Equal(shelter.Id, response.Data.First().ShelterId);
        }

        [Fact(DisplayName = "GetShelterServices - Retorna 404 cuando el ShelterId no existe")]
        public async Task GetShelterService_Returns404_WhenShelterIdNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelterService = CreateDefaultShelterService(serviceId: 1);
            context.ShelterServices.Add(shelterService);
            await context.SaveChangesAsync();

            var response = await manager.GetShelterService(999, 1);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelterServices - Retorna 404 cuando el ServiceId no existe")]
        public async Task GetShelterService_Returns404_WhenServiceIdNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelterService = CreateDefaultShelterService(shelterId: 1);
            context.ShelterServices.Add(shelterService);
            await context.SaveChangesAsync();

            var response = await manager.GetShelterService(1, 999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelterService - Retorna correctamente cuando ambos IDs existen")]
        public async Task GetShelterService_ReturnsShelterService_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelterService = CreateDefaultShelterService();
            context.ShelterServices.Add(shelterService);
            await context.SaveChangesAsync();

            var response = await manager.GetShelterService(shelterService.ShelterId, shelterService.ServiceId);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(shelterService.Description, response.Data.Description);
        }

        #endregion

        #region POST

        [Fact(DisplayName = "CreateShelterService - Retorna 404 si el ShelterService es Null")]
        public async Task CreateShelterService_Returns400_WhenNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateShelterService(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateShelterService - Retorna 404 si Shelter no existe")]
        public async Task CreateShelterService_Returns400_WhenShelterNotExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = new Shelter();
            context.Shelters.Add(shelter);
            await context.SaveChangesAsync();

            var shelterService = CreateDefaultShelterService(shelterId: shelter.Id);

            var shelterServiceDto = new ShelterServiceCreateDto
            {
                ShelterId = shelter.Id,
                ServiceId = 999,
                Price = shelterService.Price,
                IsAvailable = shelterService.IsAvailable,
                Description = shelterService.Description,
                Capacity = shelterService.Capacity
            };

            var response = await manager.CreateShelterService(shelterServiceDto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateShelterService - Retorna 404 si Service no existe")]
        public async Task CreateShelterService_Returns400_WhenServiceNotExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var service = new Service();
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var shelterService = CreateDefaultShelterService(serviceId: service.Id);

            var shelterServiceDto = new ShelterServiceCreateDto
            {
                ShelterId = 999,
                ServiceId = service.Id,
                Price = shelterService.Price,
                IsAvailable = shelterService.IsAvailable,
                Description = shelterService.Description,
                Capacity = shelterService.Capacity
            };

            var response = await manager.CreateShelterService(shelterServiceDto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateShelterService - Inserta nuevo correctamente")]
        public async Task CreateShelterService_InsertsNew()
        {
            var manager = CreateManagerWithDb(out var context);


            var shelter = new Shelter();
            context.Shelters.Add(shelter);
            var service = new Service();
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var shelterService = CreateDefaultShelterService(shelter.Id, service.Id);

            var shelterServiceDto = new ShelterServiceCreateDto
            {
                ShelterId = shelter.Id,
                ServiceId = service.Id,
                Price = shelterService.Price,
                IsAvailable = shelterService.IsAvailable,
                Description = shelterService.Description,
                Capacity = shelterService.Capacity
            };

            var response = await manager.CreateShelterService(shelterServiceDto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(shelterServiceDto.Description, response.Data.Description);

            Assert.Equal(1, context.Shelters.Count());
        }

        #endregion

        #region PUT

        [Fact(DisplayName = "UpdateShelterService - Retorna 404 si el ShelterId no existe")]
        public async Task UpdateShelterService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelterService = CreateDefaultShelterService(999);
            var shelterServiceDto = new ShelterServiceUpdateDto
            {
                ShelterId = shelterService.ShelterId,
                ServiceId = shelterService.ServiceId,
                Price = shelterService.Price,
                IsAvailable = shelterService.IsAvailable,
                Description = shelterService.Description,
                Capacity = shelterService.Capacity
            };

            var response = await manager.UpdateShelterService(shelterServiceDto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateShelterService - Actualiza correctamente un ServiceId existente")]
        public async Task UpdateShelterService_UpdatesShelter_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelterService = CreateDefaultShelterService();
            context.ShelterServices.Add(shelterService);
            await context.SaveChangesAsync();

            var shelterServiceDto = new ShelterServiceUpdateDto
            {
                ShelterId = shelterService.ShelterId,
                ServiceId = shelterService.ServiceId,
                Price = shelterService.Price,
                IsAvailable = shelterService.IsAvailable,
                Description = shelterService.Description,
                Capacity = shelterService.Capacity
            };

            var response = await manager.UpdateShelterService(shelterServiceDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(shelterServiceDto.ShelterId, response.Data.ShelterId);
            Assert.Equal(shelterServiceDto.ServiceId, response.Data.ServiceId);
            Assert.Equal(shelterServiceDto.Price, response.Data.Price);
            Assert.Equal(shelterServiceDto.IsAvailable, response.Data.IsAvailable);
            Assert.Equal(shelterServiceDto.Description, response.Data.Description);
            Assert.Equal(shelterServiceDto.Capacity, response.Data.Capacity);
        }

        [Fact(DisplayName = "UpdateShelterService - Actualiza correctamente un Shelter existente sin sobreescribir con Null Values")]
        public async Task UpdateShelterService_DoesNotUpdateNullValues_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelterService = CreateDefaultShelterService();
            context.ShelterServices.Add(shelterService);
            await context.SaveChangesAsync();

            var shelterServiceDto = new ShelterServiceUpdateDto
            {
                ShelterId = shelterService.ShelterId,
                ServiceId = shelterService.ServiceId,
                Price = null,
                IsAvailable = null,
                Description = null,
                Capacity = null
            };

            var response = await manager.UpdateShelterService(shelterServiceDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(shelterServiceDto.ShelterId, response.Data.ShelterId);
            Assert.Equal(shelterServiceDto.ServiceId, response.Data.ServiceId);
            Assert.Equal(shelterService.Price, response.Data.Price);
            Assert.Equal(shelterService.IsAvailable, response.Data.IsAvailable);
            Assert.Equal(shelterService.Description, response.Data.Description);
            Assert.Equal(shelterService.Capacity, response.Data.Capacity);
        }

        #endregion

        #region DELETE

        [Fact(DisplayName = "DeleteShelterService - Retorna 404 si el Shelter no existe")]
        public async Task DeleteShelterService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteShelterService(999, 999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteShelterService - Elimina el Shelter correctamente")]
        public async Task DeleteShelterService_DeletesShelterService_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = new Shelter();
            context.Shelters.Add(shelter);
            var service = new Service();
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var shelterService = CreateDefaultShelterService(shelter.Id, service.Id);
            context.ShelterServices.Add(shelterService);
            await context.SaveChangesAsync();

            var response = await manager.DeleteShelterService(shelter.Id, service.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(shelter.Id, response.Data.ShelterId);
            Assert.Equal(service.Id, response.Data.ServiceId);

            var deleted = await context.ShelterServices.FindAsync(shelter.Id, service.Id );
            Assert.Null(deleted);
        }

        #endregion

    }


}