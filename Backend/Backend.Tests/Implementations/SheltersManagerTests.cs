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
    public class SheltersManagerTests
    {
        private SheltersManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<SheltersManager>();
            return new SheltersManager(context, logger);
        }

        private Shelter CreateDefaultShelter(int? id = null)
        {
            var shelter = new Shelter
            {
                Name = "Test Shelter",
                Address = "Test Address",
                Latitude = 0,
                Longitude = 0,
                Phone = "1234567890",
                Capacity = 10,
                Description = "Test Description"
            };

            if (id.HasValue)
                shelter.Id = id.Value;

            return shelter;
        }

        #region GET

        [Fact(DisplayName = "GetShelters - Retorna 404 cuando no hay Shelters, y Data es Null")]
        public async Task GetShelters_ReturnsEmpty_WhenNoShelters()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetShelters();

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelters - Retorna la lista correctamente cuando hay 1 Shelter o más")]
        public async Task GetShelters_ReturnsShelters_WhenExist()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter1 = CreateDefaultShelter();
            var shelter2 = CreateDefaultShelter();
            context.Shelters.Add(shelter1);
            context.Shelters.Add(shelter2);
            await context.SaveChangesAsync();

            var response = await manager.GetShelters();

            Assert.Equal("200", response.Code);
            Assert.Equal(2, response.RowsCount);
            Assert.NotNull(response.Data);
            Assert.Equal(shelter1.Name, response.Data.First().Name);
        }

        [Fact(DisplayName = "GetShelter - Retorna 404 cuando el ID no existe")]
        public async Task GetShelter_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetShelter(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelter - Retorna correctamente cuando el ID existe")]
        public async Task GetShelter_ReturnsShelter_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = CreateDefaultShelter();
            context.Shelters.Add(shelter);
            await context.SaveChangesAsync();

            var response = await manager.GetShelter(shelter.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(shelter.Name, response.Data.Name);
        }

        #endregion

        #region POST

        [Fact(DisplayName = "CreateShelter - Retorna 404 si el Shelter es Null")]
        public async Task CreateShelter_Returns400_WhenNull()
        {  
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateShelter(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateShelter - Inserta nuevo correctamente")]
        public async Task CreateShelter_InsertsNew()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = CreateDefaultShelter();
            var shelterDto = new ShelterCreateDto
            {
                Name = shelter.Name,
                Address = shelter.Address,
                Latitude = shelter.Latitude,
                Longitude = shelter.Longitude,
                Phone = shelter.Phone,
                Capacity = shelter.Capacity,
                Description = shelter.Description
            };

            var response = await manager.CreateShelter(shelterDto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(shelterDto.Name, response.Data.Name);

            Assert.Equal(1, context.Shelters.Count());
        }

        #endregion

        #region PUT

        [Fact(DisplayName = "UpdateShelter - Retorna 404 si el Shelter no existe")]
        public async Task UpdateShelter_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = CreateDefaultShelter(999);
            var shelterDto = new ShelterUpdateDto
            {
                Id = shelter.Id,
                Name = shelter.Name,
                Address = shelter.Address,
                Latitude = shelter.Latitude,
                Longitude = shelter.Longitude,
                Phone = shelter.Phone,
                Capacity = shelter.Capacity,
                Description = shelter.Description
            };

            var response = await manager.UpdateShelter(shelterDto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateShelter - Actualiza correctamente un Shelter existente")]
        public async Task UpdateShelter_UpdatesShelter_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = CreateDefaultShelter();
            context.Shelters.Add(shelter);
            await context.SaveChangesAsync();

            var shelterDto = new ShelterUpdateDto
            {
                Id = shelter.Id,
                Name = "Updated Name",
                Address = "Updated Address",
                Latitude = 1,
                Longitude = 1,
                Phone = "0987654321",
                Capacity = 20,
                Description = "Updated Description"
            };

            var response = await manager.UpdateShelter(shelterDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(shelterDto.Id, response.Data.Id);
            Assert.Equal(shelterDto.Name, response.Data.Name);
            Assert.Equal(shelterDto.Address, response.Data.Address);
            Assert.Equal(shelterDto.Latitude, response.Data.Latitude);
            Assert.Equal(shelterDto.Latitude, response.Data.Longitude);
            Assert.Equal(shelterDto.Phone, response.Data.Phone);
            Assert.Equal(shelterDto.Capacity, response.Data.Capacity);
            Assert.Equal(shelterDto.Description, response.Data.Description);

            Assert.Equal(1, context.Shelters.Count());
        }

        [Fact(DisplayName = "UpdateShelter - Actualiza correctamente un Shelter existente sin sobreescribir con Null Values")]
        public async Task UpdateShelter_DoesNotUpdateNullValues_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = CreateDefaultShelter();
            context.Shelters.Add(shelter);
            await context.SaveChangesAsync();

            var shelterDto = new ShelterUpdateDto
            {
                Id = shelter.Id,
                Name = null,
                Address = null,
                Latitude = null,
                Longitude = null,
                Phone = null,
                Capacity = null,
                Description = null
            };

            var response = await manager.UpdateShelter(shelterDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(shelterDto.Id, response.Data.Id);
            Assert.Equal(shelter.Name, response.Data!.Name);
            Assert.Equal(shelter.Address, response.Data.Address);
            Assert.Equal(shelter.Latitude, response.Data.Latitude);
            Assert.Equal(shelter.Longitude, response.Data.Longitude);
            Assert.Equal(shelter.Phone, response.Data.Phone);
            Assert.Equal(shelter.Capacity, response.Data.Capacity);
            Assert.Equal(shelter.Description, response.Data.Description);

            Assert.Equal(1, context.Shelters.Count());
        }

        #endregion

        #region DELETE

        [Fact(DisplayName = "UpdateShelter - Retorna 404 si el Shelter no existe")]
        public async Task DeleteShelter_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteShelter(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteShelter - Elimina el Shelter correctamente")]
        public async Task DeleteShelter_DeletesShelter_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var shelter = CreateDefaultShelter();
            context.Shelters.Add(shelter);
            await context.SaveChangesAsync();

            var response = await manager.DeleteShelter(shelter.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(shelter.Id, response.Data.Id);

            var deleted = await context.Shelters.FindAsync(shelter.Id);
            Assert.Null(deleted);
        }

        #endregion

    }


}