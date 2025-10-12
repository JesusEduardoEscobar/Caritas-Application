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
            context.Beds.Add( TestEntry() );
            context.Beds.Add(new Bed { Id = 2, ShelterId = 1, BedNumber = 102, IsAvailable = true });
            context.Beds.Add(new Bed { Id = 3, ShelterId = 1, BedNumber = 103, IsAvailable = false });
            context.Beds.Add(new Bed { Id = 4, ShelterId = 2, BedNumber = 101, IsAvailable = false });
            context.Beds.Add(new Bed { Id = 5, ShelterId = 2, BedNumber = 102, IsAvailable = true });
            context.Beds.Add(new Bed { Id = 6, ShelterId = 2, BedNumber = 103, IsAvailable = true });
            context.SaveChanges();
        }

        private Bed TestEntry()
        {
            return new Bed
            {
                Id = 1,
                ShelterId = 1,
                BedNumber = 101,
                IsAvailable = true
            };
        }

        #region GET

        // ---------- GetBeds ----------

        [Fact(DisplayName = "GetBeds - Retorna 404 cuando no hay Shelters, y Data es Null")]
        public async Task GetBeds_ReturnsEmpty_WhenNoBeds()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetBeds();

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetBeds - Retorna con filtros correctamente : ShelterId")]
        public async Task GetBeds_ReturnsBeds_FilteredBy_ShelterId()
        {
            var manager = CreateManagerWithDb(out var context);

            PopulateDBContext(ref context);

            var response = await manager.GetBeds(shelterId: 1);

            Assert.Equal("200", response.Code);
            Assert.Equal(3, response.RowsCount);
            Assert.NotNull(response.Data);
        }

        [Fact(DisplayName = "GetBeds - Retorna con filtros correctamente : IsAvailable")]
        public async Task GetBeds_ReturnsBeds_FilteredBy_IsAvailable()
        {
            var manager = CreateManagerWithDb(out var context);

            PopulateDBContext(ref context);

            var response = await manager.GetBeds(available: true);

            Assert.Equal("200", response.Code);
            Assert.Equal(4, response.RowsCount);
            Assert.NotNull(response.Data);
        }

        [Fact(DisplayName = "GetBeds - Retorna con filtros correctamente : ShelterId && IsAvailable")]
        public async Task GetBeds_ReturnsBeds_FilteredBy_ShelterId_IsAvailable()
        {
            var manager = CreateManagerWithDb(out var context);

            PopulateDBContext(ref context);

            var response = await manager.GetBeds(shelterId: 1, available: true);

            Assert.Equal("200", response.Code);
            Assert.Equal(2, response.RowsCount);
            Assert.NotNull(response.Data);
        }

        // ---------- GetBed ----------

        [Fact(DisplayName = "GetShelter - Retorna 404 cuando el ID no existe")]
        public async Task GetBed_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetBed(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetShelter - Retorna correctamente cuando el ID existe")]
        public async Task GetBed_ReturnsBed_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var bed = TestEntry();

            var response = await manager.GetBed(bed.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(bed.ShelterId, response.Data.ShelterId);
            Assert.Equal(bed.BedNumber, response.Data.BedNumber);
            Assert.Equal(bed.IsAvailable, response.Data.IsAvailable);
        }

        #endregion

        #region POST

        [Fact(DisplayName = "CreateBed - Retorna 404 si el Bed es Null")]
        public async Task CreateBed_Returns400_WhenNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateBed(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateBed - Inserta nuevo correctamente")]
        public async Task CreateBed_InsertsNew()
        {
            var manager = CreateManagerWithDb(out var context);

            var bed = TestEntry();

            var bedDto = new BedCreateDto
            {
                ShelterId = bed.ShelterId,
                BedNumber = bed.BedNumber,
                IsAvailable = bed.IsAvailable
            };

            var response = await manager.CreateBed(bedDto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(bedDto.ShelterId, response.Data.ShelterId);
            Assert.Equal(bedDto.BedNumber, response.Data.BedNumber);
            Assert.Equal(bedDto.IsAvailable, response.Data.IsAvailable);

            Assert.Equal(1, context.Beds.Count());
        }

        #endregion

        #region PUT

        [Fact(DisplayName = "UpdateBed - Retorna 404 si el Bed no existe")]
        public async Task UpdateBed_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var bed = TestEntry();

            var bedDto = new BedUpdateDto
            {
                Id = bed.Id,
                ShelterId = bed.ShelterId + 1,
                BedNumber = bed.BedNumber + 1,
                IsAvailable = !bed.IsAvailable
            };

            var response = await manager.UpdateBed(bedDto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateBed - Actualiza correctamente un Bed existente")]
        public async Task UpdateBed_UpdatesBed_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var bed = TestEntry();
            context.Beds.Add(bed);
            await context.SaveChangesAsync();

            var bedDto = new BedUpdateDto
            {
                Id = bed.Id,
                ShelterId = bed.ShelterId + 1,
                BedNumber = bed.BedNumber + 1,
                IsAvailable = !bed.IsAvailable
            };

            var response = await manager.UpdateBed(bedDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(bedDto.Id, response.Data.Id);
            Assert.Equal(bedDto.ShelterId, response.Data.ShelterId);
            Assert.Equal(bedDto.BedNumber, response.Data.BedNumber);
            Assert.Equal(bedDto.IsAvailable, response.Data.IsAvailable);

            Assert.Equal(1, context.Beds.Count());
        }

        [Fact(DisplayName = "UpdateBed - Actualiza correctamente un Bed existente sin sobreescribir con Null Values")]
        public async Task UpdateBed_DoesNotUpdateNullValues_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var bed = TestEntry();
            context.Beds.Add(bed);
            await context.SaveChangesAsync();

            var bedDto = new BedUpdateDto
            {
                Id = bed.Id,
                ShelterId = null,
                BedNumber = null,
                IsAvailable = null
            };

            var response = await manager.UpdateBed(bedDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(bedDto.Id, response.Data.Id);
            Assert.Equal(bed.ShelterId, response.Data.ShelterId);
            Assert.Equal(bed.BedNumber, response.Data.BedNumber);
            Assert.Equal(bed.IsAvailable, response.Data.IsAvailable);

            Assert.Equal(1, context.Beds.Count());
        }

        #endregion

        #region PATCH

        [Fact(DisplayName = "UpdateBedAvailability - Retorna 404 si el Bed no existe")]
        public async Task UpdateBedAvailability_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var bed = TestEntry();

            var bedDto = new BedUpdateAvailabilityDto
            {
                Id = bed.Id,
                IsAvailable = !bed.IsAvailable
            };

            var response = await manager.UpdateBedAvailability(bedDto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateBedAvailability - Actualiza correctamente un Bed existente")]
        public async Task UpdateBedAvailability_UpdatesBed_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var bed = TestEntry();
            context.Beds.Add(bed);
            await context.SaveChangesAsync();

            var bedDto = new BedUpdateAvailabilityDto
            {
                Id = bed.Id,
                IsAvailable = !bed.IsAvailable
            };

            var response = await manager.UpdateBedAvailability(bedDto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);

            Assert.Equal(bedDto.Id, response.Data.Id);
            Assert.Equal(bedDto.IsAvailable, response.Data.IsAvailable);

            Assert.Equal(1, context.Beds.Count());
        }

        #endregion

        #region DELETE

        [Fact(DisplayName = "DeleteBed - Retorna 404 si el Bed no existe")]
        public async Task DeleteBed_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteBed(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteBed - Elimina el Bed correctamente")]
        public async Task DeleteBed_DeletesBed_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);

            var bed = TestEntry();
            context.Beds.Add(bed);
            await context.SaveChangesAsync();

            var response = await manager.DeleteBed(bed.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(bed.Id, response.Data.Id);

            var deleted = await context.Shelters.FindAsync(bed.Id);
            Assert.Null(deleted);
        }

        #endregion

    }


}