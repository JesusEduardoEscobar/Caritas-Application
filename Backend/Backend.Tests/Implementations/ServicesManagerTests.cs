using Backend.Dtos;
using Backend.Implementations;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using Backend.Tests.TestHelpers;
using DocumentFormat.OpenXml.InkML;
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

        private Service CreateDefaultService(int? id = null)
        {
            var service = new Service
            {
                Name = "Test Shelter",
                Description = "Test Description",
                IconKey = "test.jpg"
            };

            if (id.HasValue)
                service.Id = id.Value;

            return service;
        }

        #region GET

        [Fact(DisplayName = "GetServices - Retorna 404 cuando no hay Services, y Data es Null")]
        public async Task GetServices_ReturnsEmpty_WhenNoServices()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetServices();

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelters - Retorna la lista correctamente cuando hay 1 Shelter o más")]
        public async Task GetService_ReturnsService_WhenExist()
        {
            var manager = CreateManagerWithDb(out var context);

            var service1 = CreateDefaultService();
            var service2 = CreateDefaultService();
            context.Services.Add(service1);
            context.Services.Add(service2);
            await context.SaveChangesAsync();

            var response = await manager.GetServices();

            Assert.Equal("200", response.Code);
            Assert.Equal(2, response.RowsCount);
            Assert.Equal(service1.Name, response.Data.First().Name);
        }

        [Fact(DisplayName = "GetShelter - Retorna 404 cuando el ID no existe")]
        public async Task GetService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetService(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelter - Retorna correctamente cuando el ID existe")]
        public async Task GetService_ReturnsService_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var service = CreateDefaultService();
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var response = await manager.GetService(service.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(service.Name, response.Data.Name);
        }

        #endregion

        #region POST

        [Fact(DisplayName = "CreateService - Retorna 404 si el Service es Null")]
        public async Task CreateService_Returns400_WhenNull()
        {  
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateService(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateService - Inserta nuevo correctamente")]
        public async Task CreateService_InsertsNew()
        {
            var manager = CreateManagerWithDb(out var context);

            var service = CreateDefaultService();
            var serviceDto = new ServiceCreateDto
            {
                Name = service.Name,
                Description = service.Description,
                IconKey = service.IconKey,
            };

            var response = await manager.CreateService(serviceDto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(serviceDto.Name, response.Data.Name);

            Assert.Equal(1, context.Services.Count());
        }

        #endregion

        #region PUT

        [Fact(DisplayName = "UpdateService - Retorna 404 si el Service no existe")]
        public async Task UpdateService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var service = CreateDefaultService(999);
            var serviceDto = new ServiceUpdateDto
            {
                Id = service.Id,
                Description = service.Description,
                IconKey = service.IconKey,
            };

            var response = await manager.UpdateService(serviceDto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateService - Actualiza correctamente un Service existente")]
        public async Task UpdateService_UpdatesService_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var service = CreateDefaultService();
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var serviceDto = new ServiceUpdateDto
            {
                Id = service.Id,
                Name = "Updated Name",
                Description = "Updated Description",
                IconKey = "update.jpg",
            };

            var response = await manager.UpdateService(serviceDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(serviceDto.Id, response.Data.Id);
            Assert.Equal(serviceDto.Name, response.Data.Name);
            Assert.Equal(serviceDto.Description, response.Data.Description);
            Assert.Equal(serviceDto.IconKey, response.Data.IconKey);
        }

        [Fact(DisplayName = "UpdateService - Actualiza correctamente un Service existente sin sobreescribir con Null Values")]
        public async Task UpdateService_DoesNotUpdateNullValues_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var service = CreateDefaultService();
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var serviceDto = new ServiceUpdateDto
            {
                Id = service.Id,
                Name = null,
                Description = null,
                IconKey = null,
            };

            var response = await manager.UpdateService(serviceDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(serviceDto.Id, response.Data.Id);
            Assert.Equal(service.Name, response.Data!.Name);
            Assert.Equal(service.Description, response.Data.Description);
            Assert.Equal(service.IconKey, response.Data.IconKey);
        }

        #endregion

        #region DELETE

        [Fact(DisplayName = "UpdateService - Retorna 404 si el Service no existe")]
        public async Task DeleteService_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteService(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteService - Elimina el Service correctamente")]
        public async Task DeleteService_DeletesService_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var service = CreateDefaultService();
            context.Services.Add(service);
            await context.SaveChangesAsync();

            var response = await manager.DeleteService(service.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(service.Id, response.Data.Id);

            var deleted = await context.Services.FindAsync(service.Id);
            Assert.Null(deleted);
        }

        #endregion

    }



}