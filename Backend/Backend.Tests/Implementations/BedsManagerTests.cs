using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Models;
using Backend.Dtos;
using Backend.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class BedsManagerTests
    {
        private BedsManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<BedsManager>();
            return new BedsManager(context, logger);
        }

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });

            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.Beds.Add(new Bed { Id = 2, ShelterId = 1, BedNumber = "102", IsAvailable = true });
            context.Beds.Add(new Bed { Id = 3, ShelterId = 1, BedNumber = "103", IsAvailable = false });
            context.Beds.Add(new Bed { Id = 4, ShelterId = 2, BedNumber = "201", IsAvailable = false });
            context.Beds.Add(new Bed { Id = 5, ShelterId = 2, BedNumber = "202", IsAvailable = true });

            context.SaveChanges();
        }

        #region GetBeds

        [Fact(DisplayName = "GetBeds - Retorna 200 cuando no hay Beds")]
        public async Task GetBeds_Returns200_WhenNoBeds()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetBeds();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
        }

        [Fact(DisplayName = "GetBeds - Retorna 200 con todos los Beds")]
        public async Task GetBeds_Returns200_WithAllBeds()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetBeds();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(5, response.RowsCount);
        }

        [Fact(DisplayName = "GetBeds - Retorna 200 filtrado por ShelterId")]
        public async Task GetBeds_Returns200_FilteredByShelterId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetBeds(shelterId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.RowsCount);
            Assert.All(response.Data, bed => Assert.Equal(1, bed.ShelterId));
        }

        [Fact(DisplayName = "GetBeds - Retorna 200 filtrado por Available")]
        public async Task GetBeds_Returns200_FilteredByAvailable()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetBeds(available: true);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.RowsCount);
            Assert.All(response.Data, bed => Assert.True(bed.IsAvailable));
        }

        [Fact(DisplayName = "GetBeds - Retorna 200 filtrado por ShelterId y Available")]
        public async Task GetBeds_Returns200_FilteredByBoth()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetBeds(shelterId: 1, available: true);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.RowsCount);
        }

        #endregion

        #region GetBed

        [Fact(DisplayName = "GetBed - Retorna 404 cuando el Bed no existe")]
        public async Task GetBed_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetBed(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetBed - Retorna 200 cuando el Bed existe")]
        public async Task GetBed_Returns200_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetBed(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal("101", response.Data.BedNumber);
            Assert.True(response.Data.IsAvailable);
        }

        #endregion

        #region CreateBed

        [Fact(DisplayName = "CreateBed - Retorna 400 cuando el DTO es null")]
        public async Task CreateBed_Returns400_WhenDtoIsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateBed(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateBed - Retorna 404 cuando el Shelter no existe")]
        public async Task CreateBed_Returns404_WhenShelterNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new BedCreateDto
            {
                ShelterId = 999,
                BedNumber = "101",
                IsAvailable = true
            };

            var response = await manager.CreateBed(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateBed - Retorna 200 cuando se crea correctamente")]
        public async Task CreateBed_Returns200_WhenCreatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.SaveChanges();

            var dto = new BedCreateDto
            {
                ShelterId = 1,
                BedNumber = "101",
                IsAvailable = true
            };

            var response = await manager.CreateBed(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal("101", response.Data.BedNumber);
            Assert.True(response.Data.IsAvailable);
        }

        #endregion

        #region UpdateBed

        [Fact(DisplayName = "UpdateBed - Retorna 404 cuando el Bed no existe")]
        public async Task UpdateBed_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new BedPutDto
            {
                Id = 999,
                ShelterId = 1,
                BedNumber = "101",
                IsAvailable = true
            };

            var response = await manager.UpdateBed(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateBed - Retorna 404 cuando el Shelter no existe")]
        public async Task UpdateBed_Returns404_WhenShelterNotFound()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.SaveChanges();

            var dto = new BedPutDto
            {
                Id = 1,
                ShelterId = 999,
                BedNumber = "102",
                IsAvailable = false
            };

            var response = await manager.UpdateBed(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateBed - Retorna 200 cuando se actualiza correctamente")]
        public async Task UpdateBed_Returns200_WhenUpdatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });
            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.SaveChanges();

            var dto = new BedPutDto
            {
                Id = 1,
                ShelterId = 2,
                BedNumber = "202",
                IsAvailable = false
            };

            var response = await manager.UpdateBed(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.ShelterId);
            Assert.Equal("202", response.Data.BedNumber);
            Assert.False(response.Data.IsAvailable);
        }

        #endregion

        #region UpdateBedAvailability

        [Fact(DisplayName = "UpdateBedAvailability - Retorna 404 cuando el Bed no existe")]
        public async Task UpdateBedAvailability_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new BedPatchAvailabilityDto
            {
                Id = 999,
                IsAvailable = false
            };

            var response = await manager.UpdateBedAvailability(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateBedAvailability - Retorna 200 cuando se actualiza correctamente")]
        public async Task UpdateBedAvailability_Returns200_WhenUpdatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.SaveChanges();

            var dto = new BedPatchAvailabilityDto
            {
                Id = 1,
                IsAvailable = false
            };

            var response = await manager.UpdateBedAvailability(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.False(response.Data.IsAvailable);
        }

        #endregion

        #region DeleteBed

        [Fact(DisplayName = "DeleteBed - Retorna 404 cuando el Bed no existe")]
        public async Task DeleteBed_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteBed(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteBed - Retorna 200 cuando se elimina correctamente")]
        public async Task DeleteBed_Returns200_WhenDeletedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.SaveChanges();

            var response = await manager.DeleteBed(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);

            var deleted = await context.Beds.FindAsync(1);
            Assert.Null(deleted);
        }

        #endregion
    }
}