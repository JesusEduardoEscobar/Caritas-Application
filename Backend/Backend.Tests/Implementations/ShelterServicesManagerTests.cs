using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Models;
using Backend.Dtos;
using Backend.Tests.TestHelpers;
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

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });

            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.Services.Add(new Service { Id = 2, Name = "Duchas" });
            context.Services.Add(new Service { Id = 3, Name = "Lavandería" });

            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 1,
                Price = 50,
                IsAvailable = true,
                Description = "Tres comidas al día",
                Capacity = 100
            });

            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 2,
                Price = 30,
                IsAvailable = true,
                Description = "Duchas con agua caliente",
                Capacity = 20
            });

            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 2,
                ServiceId = 1,
                Price = 40,
                IsAvailable = false,
                Description = "Servicio de comedor",
                Capacity = 50
            });

            context.SaveChanges();
        }

        #region GetShelterServices

        [Fact(DisplayName = "GetShelterServices - Retorna 200 cuando no hay ShelterServices")]
        public async Task GetShelterServices_Returns200_WhenNoShelterServices()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetShelterServices();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
        }

        [Fact(DisplayName = "GetShelterServices - Retorna 200 con todos los ShelterServices")]
        public async Task GetShelterServices_Returns200_WithAllShelterServices()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetShelterServices();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.RowsCount);
        }

        #endregion

        #region GetShelterServicesByShelter

        [Fact(DisplayName = "GetShelterServicesByShelter - Retorna 200 cuando no hay ShelterServices para el Shelter")]
        public async Task GetShelterServicesByShelter_Returns200_WhenNoServicesForShelter()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Shelters.Add(new Shelter { Id = 99, Name = "Empty Shelter" });
            context.SaveChanges();

            var response = await manager.GetShelterServicesByShelter(99);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
        }

        [Fact(DisplayName = "GetShelterServicesByShelter - Retorna 200 con los ShelterServices del Shelter")]
        public async Task GetShelterServicesByShelter_Returns200_WithShelterServices()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetShelterServicesByShelter(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.RowsCount);
            Assert.All(response.Data, ss => Assert.Equal(1, ss.ShelterId));
        }

        #endregion

        #region GetShelterService

        [Fact(DisplayName = "GetShelterService - Retorna 404 cuando el ShelterService no existe")]
        public async Task GetShelterService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetShelterService(999, 999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelterService - Retorna 200 cuando el ShelterService existe")]
        public async Task GetShelterService_Returns200_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetShelterService(1, 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal(1, response.Data.ServiceId);
            Assert.Equal(50, response.Data.Price);
            Assert.True(response.Data.IsAvailable);
            Assert.Equal("Tres comidas al día", response.Data.Description);
            Assert.Equal(100, response.Data.Capacity);
        }

        #endregion

        #region CreateShelterService

        [Fact(DisplayName = "CreateShelterService - Retorna 400 cuando el DTO es null")]
        public async Task CreateShelterService_Returns400_WhenDtoIsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateShelterService(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateShelterService - Retorna 404 cuando el Shelter no existe")]
        public async Task CreateShelterService_Returns404_WhenShelterNotFound()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.SaveChanges();

            var dto = new ShelterServiceCreateDto
            {
                ShelterId = 999,
                ServiceId = 1,
                Price = 50,
                IsAvailable = true,
                Capacity = 10
            };

            var response = await manager.CreateShelterService(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateShelterService - Retorna 404 cuando el Service no existe")]
        public async Task CreateShelterService_Returns404_WhenServiceNotFound()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.SaveChanges();

            var dto = new ShelterServiceCreateDto
            {
                ShelterId = 1,
                ServiceId = 999,
                Price = 50,
                IsAvailable = true,
                Capacity = 10
            };

            var response = await manager.CreateShelterService(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateShelterService - Retorna 201 cuando se crea correctamente")]
        public async Task CreateShelterService_Returns201_WhenCreatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.SaveChanges();

            var dto = new ShelterServiceCreateDto
            {
                ShelterId = 1,
                ServiceId = 1,
                Price = 50,
                IsAvailable = true,
                Description = "Servicio de comidas",
                Capacity = 100
            };

            var response = await manager.CreateShelterService(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal(1, response.Data.ServiceId);
            Assert.Equal(50, response.Data.Price);
            Assert.True(response.Data.IsAvailable);
            Assert.Equal("Servicio de comidas", response.Data.Description);
            Assert.Equal(100, response.Data.Capacity);
        }

        #endregion

        #region UpdateShelterService

        [Fact(DisplayName = "UpdateShelterService - Retorna 404 cuando el ShelterService no existe")]
        public async Task UpdateShelterService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ShelterServicePutDto
            {
                ShelterId = 999,
                ServiceId = 999,
                Price = 60,
                IsAvailable = false,
                Description = "Actualizado",
                Capacity = 50
            };

            var response = await manager.UpdateShelterService(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateShelterService - Retorna 200 cuando se actualiza correctamente")]
        public async Task UpdateShelterService_Returns200_WhenUpdatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var dto = new ShelterServicePutDto
            {
                ShelterId = 1,
                ServiceId = 1,
                Price = 60,
                IsAvailable = false,
                Description = "Actualizado",
                Capacity = 150
            };

            var response = await manager.UpdateShelterService(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal(1, response.Data.ServiceId);
            Assert.Equal(60, response.Data.Price);
            Assert.False(response.Data.IsAvailable);
            Assert.Equal("Actualizado", response.Data.Description);
            Assert.Equal(150, response.Data.Capacity);
        }

        #endregion

        #region DeleteShelterService

        [Fact(DisplayName = "DeleteShelterService - Retorna 404 cuando el ShelterService no existe")]
        public async Task DeleteShelterService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteShelterService(999, 999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteShelterService - Retorna 200 cuando se elimina correctamente")]
        public async Task DeleteShelterService_Returns200_WhenDeletedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.DeleteShelterService(1, 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal(1, response.Data.ServiceId);

            var deleted = await context.ShelterServices.FindAsync(1, 1);
            Assert.Null(deleted);
        }

        #endregion
    }
}