using Backend.Dtos;
using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class TransportRequestsManagerTests
    {
        private TransportRequestsManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<TransportRequestsManager>();
            return new TransportRequestsManager(context, logger);
        }

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Users.Add(new User { Id = 2, Name = "User 2" });

            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });

            context.Cars.Add(new Car { Id = 1, ShelterId = 1, Model = "Car 1" });
            context.Cars.Add(new Car { Id = 2, ShelterId = 1, Model = "Car 2" });
            context.Cars.Add(new Car { Id = 3, ShelterId = 2, Model = "Car 3" });

            var futureDate1 = DateTime.UtcNow.Date.AddDays(1);
            var futureDate2 = DateTime.UtcNow.Date.AddDays(2);
            var futureDate3 = DateTime.UtcNow.Date.AddDays(3);

            context.TransportRequests.Add(new TransportRequest
            {
                Id = 1,
                UserId = 1,
                CarId = 1,
                PickupLocation = "Location A",
                DropoffLocation = "Location B",
                RequestDate = futureDate1,
                Status = ReservationStatus.reserved
            });

            context.TransportRequests.Add(new TransportRequest
            {
                Id = 2,
                UserId = 2,
                CarId = 2,
                PickupLocation = "Location C",
                DropoffLocation = "Location D",
                RequestDate = futureDate2,
                Status = ReservationStatus.checked_in
            });

            context.TransportRequests.Add(new TransportRequest
            {
                Id = 3,
                UserId = 1,
                CarId = 3,
                PickupLocation = "Location E",
                DropoffLocation = "Location F",
                RequestDate = futureDate3,
                Status = ReservationStatus.completed
            });

            context.SaveChanges();
        }

        private TransportRequest TestEntry()
        {
            return new TransportRequest
            {
                Id = 1,
                UserId = 1,
                CarId = 1,
                PickupLocation = "Location A",
                DropoffLocation = "Location B",
                RequestDate = DateTime.UtcNow.Date.AddDays(1),
                Status = ReservationStatus.reserved
            };
        }

        #region GET


        [Fact(DisplayName = "GetTransportRequests - Retorna lista vacía cuando no hay TransportRequests")]
        public async Task GetTransportRequests_ReturnsEmpty_WhenNoTransportRequests()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetTransportRequests();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Equal(0, response.RowsCount);
        }

        [Fact(DisplayName = "GetTransportRequests - Retorna todas las peticiones correctamente")]
        public async Task GetTransportRequests_ReturnsAll_WhenNoFilters()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetTransportRequests();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.RowsCount);
        }

        [Fact(DisplayName = "GetTransportRequests - Retorna con filtro correctamente: UserId")]
        public async Task GetTransportRequests_ReturnsFiltered_ByUserId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetTransportRequests(userId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.RowsCount);
            Assert.All(response.Data, tr => Assert.Equal(1, tr.UserId));
        }

        [Fact(DisplayName = "GetTransportRequests - Retorna con filtro correctamente: ShelterId")]
        public async Task GetTransportRequests_ReturnsFiltered_ByShelterId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetTransportRequests(shelterId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.RowsCount);
        }

        [Fact(DisplayName = "GetTransportRequests - Retorna con filtro correctamente: RequestDate")]
        public async Task GetTransportRequests_ReturnsFiltered_ByRequestDate()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var filterDate = DateTime.UtcNow.Date.AddDays(1);
            var response = await manager.GetTransportRequests(requestDate: filterDate);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
        }

        [Fact(DisplayName = "GetTransportRequests - Retorna con filtro correctamente: Status")]
        public async Task GetTransportRequests_ReturnsFiltered_ByStatus()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetTransportRequests(status: ReservationStatus.reserved);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
            Assert.All(response.Data, tr => Assert.Equal(ReservationStatus.reserved, tr.Status));
        }

        [Fact(DisplayName = "GetTransportRequests - Retorna con múltiples filtros correctamente")]
        public async Task GetTransportRequests_ReturnsFiltered_ByMultipleFilters()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetTransportRequests(
                userId: 1,
                shelterId: 1,
                status: ReservationStatus.reserved
            );

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
        }

        // ---------- GetTransportRequest ----------

        [Fact(DisplayName = "GetTransportRequest - Retorna 404 cuando el ID no existe")]
        public async Task GetTransportRequest_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetTransportRequest(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetTransportRequest - Retorna correctamente cuando el ID existe")]
        public async Task GetTransportRequest_ReturnsTransportRequest_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var transportRequest = TestEntry();

            var response = await manager.GetTransportRequest(transportRequest.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(transportRequest.Id, response.Data.Id);
            Assert.Equal(transportRequest.UserId, response.Data.UserId);
            Assert.Equal(transportRequest.CarId, response.Data.CarId);
            Assert.Equal(transportRequest.PickupLocation, response.Data.PickupLocation);
            Assert.Equal(transportRequest.DropoffLocation, response.Data.DropoffLocation);
        }

        #endregion

        #region POST

        [Fact(DisplayName = "CreateTransportRequest - Retorna 400 si el DTO es null")]
        public async Task CreateTransportRequest_Returns400_WhenDtoIsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateTransportRequest(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateTransportRequest - Retorna 400 si la fecha es pasada o actual")]
        public async Task CreateTransportRequest_Returns400_WhenDateIsNotFuture()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new TransportRequestCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                PickupLocation = "Location A",
                DropoffLocation = "Location B",
                RequestDate = DateTime.UtcNow.Date // Fecha de hoy
            };

            var response = await manager.CreateTransportRequest(dto);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateTransportRequest - Retorna 404 si el Usuario no existe")]
        public async Task CreateTransportRequest_Returns404_WhenUserNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            context.Shelters.Add(new Shelter { Id = 1 });
            context.Cars.Add(new Car { Id = 1, ShelterId = 1 });
            await context.SaveChangesAsync();

            var dto = new TransportRequestCreateDto
            {
                UserId = 999,
                ShelterId = 1,
                PickupLocation = "Location A",
                DropoffLocation = "Location B",
                RequestDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateTransportRequest(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateTransportRequest - Retorna 409 cuando no hay carros disponibles")]
        public async Task CreateTransportRequest_Returns409_WhenNoAvailableCars()
        {
            var manager = CreateManagerWithDb(out var context);

            context.Users.Add(new User { Id = 1 });
            context.Shelters.Add(new Shelter { Id = 1 });
            await context.SaveChangesAsync();

            var dto = new TransportRequestCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                PickupLocation = "Location A",
                DropoffLocation = "Location B",
                RequestDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateTransportRequest(dto);

            Assert.Equal("409", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateTransportRequest - Crea correctamente una nueva petición")]
        public async Task CreateTransportRequest_CreatesSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);

            context.Users.Add(new User { Id = 1 });
            context.Shelters.Add(new Shelter { Id = 1 });
            context.Cars.Add(new Car { Id = 1, ShelterId = 1 });
            await context.SaveChangesAsync();

            var dto = new TransportRequestCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                PickupLocation = "Location A",
                DropoffLocation = "Location B",
                RequestDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateTransportRequest(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(dto.UserId, response.Data.UserId);
            Assert.Equal(dto.PickupLocation, response.Data.PickupLocation);
            Assert.Equal(dto.DropoffLocation, response.Data.DropoffLocation);
            Assert.Equal(ReservationStatus.reserved, response.Data.Status);
            Assert.Equal(1, context.TransportRequests.Count());
        }

        [Fact(DisplayName = "CreateTransportRequest - Asigna el carro con menos peticiones")]
        public async Task CreateTransportRequest_AssignsCarWithLeastRequests()
        {
            var manager = CreateManagerWithDb(out var context);

            context.Users.Add(new User { Id = 1 });
            context.Shelters.Add(new Shelter { Id = 1 });
            context.Cars.Add(new Car { Id = 1, ShelterId = 1 });
            context.Cars.Add(new Car { Id = 2, ShelterId = 1 });

            var requestDate = DateTime.UtcNow.Date.AddDays(1);

            // Car 1 ya tiene una petición
            context.TransportRequests.Add(new TransportRequest
            {
                Id = 1,
                UserId = 1,
                CarId = 1,
                PickupLocation = "Test",
                DropoffLocation = "Test",
                RequestDate = requestDate,
                Status = ReservationStatus.reserved
            });

            await context.SaveChangesAsync();

            var dto = new TransportRequestCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                PickupLocation = "Location A",
                DropoffLocation = "Location B",
                RequestDate = requestDate
            };

            var response = await manager.CreateTransportRequest(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.CarId); // Debe asignar Car 2
        }

        #endregion

        #region PATCH

        // ---------- UpdateTransportRequest ----------

        [Fact(DisplayName = "UpdateTransportRequest - Retorna 400 si la fecha es pasada o actual")]
        public async Task UpdateTransportRequest_Returns400_WhenDateIsNotFuture()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new TransportRequestPatchDto
            {
                Id = 1,
                PickupLocation = "New Location",
                DropoffLocation = "New Location",
                RequestDate = DateTime.UtcNow.Date
            };

            var response = await manager.UpdateTransportRequest(dto);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateTransportRequest - Retorna 404 si la petición no existe")]
        public async Task UpdateTransportRequest_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new TransportRequestPatchDto
            {
                Id = 999,
                PickupLocation = "New Location",
                DropoffLocation = "New Location",
                RequestDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.UpdateTransportRequest(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateTransportRequest - Actualiza correctamente una petición existente")]
        public async Task UpdateTransportRequest_UpdatesSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var newDate = DateTime.UtcNow.Date.AddDays(5);
            var dto = new TransportRequestPatchDto
            {
                Id = 1,
                PickupLocation = "Updated Pickup",
                DropoffLocation = "Updated Dropoff",
                RequestDate = newDate
            };

            var response = await manager.UpdateTransportRequest(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(dto.Id, response.Data.Id);
            Assert.Equal(dto.PickupLocation, response.Data.PickupLocation);
            Assert.Equal(dto.DropoffLocation, response.Data.DropoffLocation);
            Assert.Equal(newDate, response.Data.RequestDate);
        }

        // ---------- UpdateTransportRequestStatus ----------

        [Fact(DisplayName = "UpdateTransportRequestStatus - Retorna 404 si la petición no existe")]
        public async Task UpdateTransportRequestStatus_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new TransportRequestPatchStatusDto
            {
                Id = 999,
                Status = ReservationStatus.completed
            };

            var response = await manager.UpdateTransportRequestStatus(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateTransportRequestStatus - Actualiza el status correctamente")]
        public async Task UpdateTransportRequestStatus_UpdatesStatusSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var dto = new TransportRequestPatchStatusDto
            {
                Id = 1,
                Status = ReservationStatus.completed
            };

            var response = await manager.UpdateTransportRequestStatus(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(dto.Id, response.Data.Id);
            Assert.Equal(ReservationStatus.completed, response.Data.Status);
        }

        #endregion

        #region DELETE

        [Fact(DisplayName = "DeleteTransportRequest - Retorna 404 si la petición no existe")]
        public async Task DeleteTransportRequest_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteTransportRequest(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteTransportRequest - Elimina correctamente una petición existente")]
        public async Task DeleteTransportRequest_DeletesSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.DeleteTransportRequest(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);

            var deleted = await context.TransportRequests.FindAsync(1);
            Assert.Null(deleted);
        }

        #endregion
    }
}