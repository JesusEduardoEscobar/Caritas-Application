using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Models;
using Backend.Dtos;
using Backend.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class ServicesManagerTests
    {
        private ServicesManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<ServicesManager>();
            return new ServicesManager(context, logger);
        }

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            context.Services.Add(new Service { Id = 1, Name = "Comida", Description = "Servicio de comidas", IconKey = "food.png" });
            context.Services.Add(new Service { Id = 2, Name = "Duchas", Description = "Servicio de duchas", IconKey = "shower.png" });
            context.Services.Add(new Service { Id = 3, Name = "Lavandería", Description = "Servicio de lavandería", IconKey = "laundry.png" });

            context.SaveChanges();
        }

        #region GetServices

        [Fact(DisplayName = "GetServices - Retorna 200 cuando no hay Services")]
        public async Task GetServices_Returns200_WhenNoServices()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetServices();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
        }

        [Fact(DisplayName = "GetServices - Retorna 200 con todos los Services")]
        public async Task GetServices_Returns200_WithAllServices()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServices();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.RowsCount);
        }

        #endregion

        #region GetService

        [Fact(DisplayName = "GetService - Retorna 404 cuando el Service no existe")]
        public async Task GetService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetService(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetService - Retorna 200 cuando el Service existe")]
        public async Task GetService_Returns200_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetService(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal("Comida", response.Data.Name);
            Assert.Equal("Servicio de comidas", response.Data.Description);
            Assert.Equal("food.png", response.Data.IconKey);
        }

        #endregion

        #region CreateService

        [Fact(DisplayName = "CreateService - Retorna 400 cuando el DTO es null")]
        public async Task CreateService_Returns400_WhenDtoIsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateService(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateService - Retorna 201 cuando se crea correctamente")]
        public async Task CreateService_Returns201_WhenCreatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ServiceCreateDto
            {
                Name = "Internet",
                Description = "Servicio de WiFi",
                IconKey = "wifi.png"
            };

            var response = await manager.CreateService(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("Internet", response.Data.Name);
            Assert.Equal("Servicio de WiFi", response.Data.Description);
            Assert.Equal("wifi.png", response.Data.IconKey);
        }

        [Fact(DisplayName = "CreateService - Retorna 201 con campos opcionales null")]
        public async Task CreateService_Returns201_WithOptionalFieldsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ServiceCreateDto
            {
                Name = "Asesoría",
                Description = null,
                IconKey = null
            };

            var response = await manager.CreateService(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("Asesoría", response.Data.Name);
            Assert.Null(response.Data.Description);
            Assert.Null(response.Data.IconKey);
        }

        #endregion

        #region UpdateService

        [Fact(DisplayName = "UpdateService - Retorna 404 cuando el Service no existe")]
        public async Task UpdateService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ServicePutDto
            {
                Id = 999,
                Name = "Servicio Actualizado",
                Description = "Nueva descripción",
                IconKey = "new.png"
            };

            var response = await manager.UpdateService(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateService - Retorna 200 cuando se actualiza correctamente")]
        public async Task UpdateService_Returns200_WhenUpdatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var dto = new ServicePutDto
            {
                Id = 1,
                Name = "Comida Actualizada",
                Description = "Descripción actualizada",
                IconKey = "food_new.png"
            };

            var response = await manager.UpdateService(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("Comida Actualizada", response.Data.Name);
            Assert.Equal("Descripción actualizada", response.Data.Description);
            Assert.Equal("food_new.png", response.Data.IconKey);
        }

        #endregion

        #region DeleteService

        [Fact(DisplayName = "DeleteService - Retorna 404 cuando el Service no existe")]
        public async Task DeleteService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteService(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteService - Retorna 200 cuando se elimina correctamente")]
        public async Task DeleteService_Returns200_WhenDeletedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.DeleteService(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);

            var deleted = await context.Services.FindAsync(1);
            Assert.Null(deleted);
        }

        #endregion
    }
}