using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Models;
using Backend.Dtos;
using Backend.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class ReservationsManagerTests
    {
        private ReservationsManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<ReservationsManager>();
            return new ReservationsManager(context, logger);
        }

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Users.Add(new User { Id = 2, Name = "User 2" });

            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });

            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.Beds.Add(new Bed { Id = 2, ShelterId = 1, BedNumber = "102", IsAvailable = true });
            context.Beds.Add(new Bed { Id = 3, ShelterId = 2, BedNumber = "201", IsAvailable = false });

            var futureDate1 = DateTime.UtcNow.Date.AddDays(1);
            var futureDate2 = DateTime.UtcNow.Date.AddDays(5);

            context.Reservations.Add(new Reservation
            {
                Id = 1,
                UserId = 1,
                BedId = 1,
                StartDate = futureDate1,
                EndDate = futureDate1.AddDays(2),
                Status = ReservationStatus.reserved,
                QrData = "QR-1"
            });

            context.Reservations.Add(new Reservation
            {
                Id = 2,
                UserId = 2,
                BedId = 3,
                StartDate = futureDate2,
                EndDate = futureDate2.AddDays(3),
                Status = ReservationStatus.checked_in,
                QrData = "QR-2"
            });

            context.SaveChanges();
        }

        #region GetReservations

        [Fact(DisplayName = "GetReservations - Retorna 200 con lista vacía cuando no hay Reservations")]
        public async Task GetReservations_Returns200_WhenNoReservations()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetReservations();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Equal(0, response.RowsCount);
        }

        [Fact(DisplayName = "GetReservations - Retorna 200 con todas las Reservations")]
        public async Task GetReservations_Returns200_WithAllReservations()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetReservations();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.RowsCount);
        }

        [Fact(DisplayName = "GetReservations - Retorna 200 filtrado por ShelterId")]
        public async Task GetReservations_Returns200_FilteredByShelterId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetReservations(shelterId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
        }

        [Fact(DisplayName = "GetReservations - Retorna 200 filtrado por UserId")]
        public async Task GetReservations_Returns200_FilteredByUserId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetReservations(userId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
            Assert.All(response.Data, r => Assert.Equal(1, r.UserId));
        }

        [Fact(DisplayName = "GetReservations - Retorna 200 filtrado por Status")]
        public async Task GetReservations_Returns200_FilteredByStatus()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetReservations(status: ReservationStatus.reserved);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
            Assert.All(response.Data, r => Assert.Equal(ReservationStatus.reserved, r.Status));
        }

        [Fact(DisplayName = "GetReservations - Retorna 200 filtrado por Date")]
        public async Task GetReservations_Returns200_FilteredByDate()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var filterDate = DateTime.UtcNow.Date.AddDays(1);
            var response = await manager.GetReservations(date: filterDate);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
        }

        #endregion

        #region GetReservation

        [Fact(DisplayName = "GetReservation - Retorna 404 cuando la Reservation no existe")]
        public async Task GetReservation_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetReservation(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetReservation - Retorna 200 cuando la Reservation existe")]
        public async Task GetReservation_Returns200_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetReservation(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal(1, response.Data.UserId);
            Assert.Equal(1, response.Data.BedId);
        }

        [Fact(DisplayName = "GetReservation - Retorna 404 cuando el QrData no existe")]
        public async Task GetReservationByQr_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetReservation("QR-999");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetReservation - Retorna 200 cuando el QrData existe")]
        public async Task GetReservationByQr_Returns200_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetReservation("QR-1");

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal("QR-1", response.Data.QrData);
        }

        #endregion

        #region CreateReservation

        [Fact(DisplayName = "CreateReservation - Retorna 400 cuando el DTO es null")]
        public async Task CreateReservation_Returns400_WhenDtoIsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateReservation(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateReservation - Retorna 404 cuando la fecha no es futura")]
        public async Task CreateReservation_Returns404_WhenDateIsNotFuture()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ReservationCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateReservation(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateReservation - Retorna 404 cuando el User no existe")]
        public async Task CreateReservation_Returns404_WhenUserNotFound()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.SaveChanges();

            var dto = new ReservationCreateDto
            {
                UserId = 999,
                ShelterId = 1,
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(2)
            };

            var response = await manager.CreateReservation(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateReservation - Retorna 409 cuando no hay camas disponibles")]
        public async Task CreateReservation_Returns409_WhenNoBedsAvailable()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.SaveChanges();

            var dto = new ReservationCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(2)
            };

            var response = await manager.CreateReservation(dto);

            Assert.Equal("409", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateReservation - Retorna 201 cuando se crea correctamente")]
        public async Task CreateReservation_Returns201_WhenCreatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Beds.Add(new Bed { Id = 1, ShelterId = 1, BedNumber = "101", IsAvailable = true });
            context.SaveChanges();

            var dto = new ReservationCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(2)
            };

            var response = await manager.CreateReservation(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.UserId);
            Assert.Equal(ReservationStatus.reserved, response.Data.Status);
            Assert.NotEmpty(response.Data.QrData);
        }

        #endregion

        #region UpdateReservationStatus

        [Fact(DisplayName = "UpdateReservationStatus - Retorna 404 cuando la Reservation no existe")]
        public async Task UpdateReservationStatus_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ReservationPatchStatusDto
            {
                Id = 999,
                Status = ReservationStatus.completed
            };

            var response = await manager.UpdateReservationStatus(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateReservationStatus - Retorna 200 cuando se actualiza correctamente")]
        public async Task UpdateReservationStatus_Returns200_WhenUpdatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var dto = new ReservationPatchStatusDto
            {
                Id = 1,
                Status = ReservationStatus.completed
            };

            var response = await manager.UpdateReservationStatus(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(ReservationStatus.completed, response.Data.Status);
        }

        #endregion

        #region UpdateReservationPeriod

        [Fact(DisplayName = "UpdateReservationPeriod - Retorna 404 cuando la fecha no es futura")]
        public async Task UpdateReservationPeriod_Returns404_WhenDateIsNotFuture()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ReservationPatchPeriodDto
            {
                Id = 1,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.UpdateReservationPeriod(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateReservationPeriod - Retorna 404 cuando la Reservation no existe")]
        public async Task UpdateReservationPeriod_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ReservationPatchPeriodDto
            {
                Id = 999,
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(3)
            };

            var response = await manager.UpdateReservationPeriod(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateReservationPeriod - Retorna 200 cuando se actualiza correctamente")]
        public async Task UpdateReservationPeriod_Returns200_WhenUpdatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var newStartDate = DateTime.UtcNow.Date.AddDays(5);
            var newEndDate = DateTime.UtcNow.Date.AddDays(7);

            var dto = new ReservationPatchPeriodDto
            {
                Id = 1,
                StartDate = newStartDate,
                EndDate = newEndDate
            };

            var response = await manager.UpdateReservationPeriod(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(newStartDate, response.Data.StartDate);
            Assert.Equal(newEndDate, response.Data.EndDate);
        }

        #endregion

        #region DeleteReservation

        [Fact(DisplayName = "DeleteReservation - Retorna 404 cuando la Reservation no existe")]
        public async Task DeleteReservation_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteReservation(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteReservation - Retorna 200 cuando se elimina correctamente")]
        public async Task DeleteReservation_Returns200_WhenDeletedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.DeleteReservation(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);

            var deleted = await context.Reservations.FindAsync(1);
            Assert.Null(deleted);
        }

        #endregion
    }
}