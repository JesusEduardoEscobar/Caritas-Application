using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Models;
using Backend.Dtos;
using Backend.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class ServiceReservationsManagerTests
    {
        private ServiceReservationsManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<ServiceReservationsManager>();
            return new ServiceReservationsManager(context, logger);
        }

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Users.Add(new User { Id = 2, Name = "User 2" });

            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });

            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.Services.Add(new Service { Id = 2, Name = "Duchas" });

            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 1,
                Price = 50,
                IsAvailable = true,
                Capacity = 10
            });

            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 2,
                Price = 30,
                IsAvailable = true,
                Capacity = 5
            });

            var serviceDate1 = DateTime.UtcNow.Date.AddDays(1);
            var serviceDate2 = DateTime.UtcNow.Date.AddDays(2);

            context.ServiceReservations.Add(new ServiceReservation
            {
                Id = 1,
                UserId = 1,
                ShelterId = 1,
                ServiceId = 1,
                QrData = "SERVICE-QR-1",
                ServiceDate = serviceDate1,
                IsActive = true
            });

            context.ServiceReservations.Add(new ServiceReservation
            {
                Id = 2,
                UserId = 2,
                ShelterId = 1,
                ServiceId = 2,
                QrData = "SERVICE-QR-2",
                ServiceDate = serviceDate2,
                IsActive = false
            });

            context.SaveChanges();
        }

        #region GetServiceReservations

        [Fact(DisplayName = "GetServiceReservations - Retorna 200 con lista vacía cuando no hay ServiceReservations")]
        public async Task GetServiceReservations_Returns200_WhenNoReservations()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetServiceReservations();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Equal(0, response.RowsCount);
        }

        [Fact(DisplayName = "GetServiceReservations - Retorna 200 con todas las ServiceReservations")]
        public async Task GetServiceReservations_Returns200_WithAllReservations()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServiceReservations();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.RowsCount);
        }

        [Fact(DisplayName = "GetServiceReservations - Retorna 200 filtrado por UserId")]
        public async Task GetServiceReservations_Returns200_FilteredByUserId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServiceReservations(userId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
            Assert.All(response.Data, sr => Assert.Equal(1, sr.UserId));
        }

        [Fact(DisplayName = "GetServiceReservations - Retorna 200 filtrado por ShelterId")]
        public async Task GetServiceReservations_Returns200_FilteredByShelterId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServiceReservations(shelterId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.RowsCount);
            Assert.All(response.Data, sr => Assert.Equal(1, sr.ShelterId));
        }

        [Fact(DisplayName = "GetServiceReservations - Retorna 200 filtrado por ServiceId")]
        public async Task GetServiceReservations_Returns200_FilteredByServiceId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServiceReservations(serviceId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
            Assert.All(response.Data, sr => Assert.Equal(1, sr.ServiceId));
        }

        [Fact(DisplayName = "GetServiceReservations - Retorna 200 filtrado por IsActive")]
        public async Task GetServiceReservations_Returns200_FilteredByIsActive()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServiceReservations(isActive: true);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.RowsCount);
            Assert.All(response.Data, sr => Assert.True(sr.IsActive));
        }

        #endregion

        #region GetServiceReservation

        [Fact(DisplayName = "GetServiceReservation - Retorna 404 cuando la ServiceReservation no existe")]
        public async Task GetServiceReservation_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetServiceReservation(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetServiceReservation - Retorna 200 cuando la ServiceReservation existe")]
        public async Task GetServiceReservation_Returns200_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServiceReservation(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal(1, response.Data.UserId);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal(1, response.Data.ServiceId);
        }

        [Fact(DisplayName = "GetServiceReservation - Retorna 404 cuando el QrData no existe")]
        public async Task GetServiceReservationByQr_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetServiceReservation("INVALID-QR");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetServiceReservation - Retorna 200 cuando el QrData existe")]
        public async Task GetServiceReservationByQr_Returns200_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetServiceReservation("SERVICE-QR-1");

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("SERVICE-QR-1", response.Data.QrData);
        }

        #endregion

        #region CreateServiceReservation

        [Fact(DisplayName = "CreateServiceReservation - Retorna 400 cuando el DTO es null")]
        public async Task CreateServiceReservation_Returns400_WhenDtoIsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateServiceReservation(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateServiceReservation - Retorna 404 cuando el User no existe")]
        public async Task CreateServiceReservation_Returns404_WhenUserNotFound()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 1,
                IsAvailable = true,
                Capacity = 10
            });
            context.SaveChanges();

            var dto = new ServiceReservationCreateDto
            {
                UserId = 999,
                ShelterId = 1,
                ServiceId = 1,
                ServiceDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateServiceReservation(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateServiceReservation - Retorna 404 cuando el ShelterService no existe")]
        public async Task CreateServiceReservation_Returns404_WhenShelterServiceNotFound()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.SaveChanges();

            var dto = new ServiceReservationCreateDto
            {
                UserId = 1,
                ShelterId = 999,
                ServiceId = 999,
                ServiceDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateServiceReservation(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateServiceReservation - Retorna 404 cuando el ShelterService no está activo")]
        public async Task CreateServiceReservation_Returns404_WhenShelterServiceNotAvailable()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 1,
                IsAvailable = false,
                Capacity = 10
            });
            context.SaveChanges();

            var dto = new ServiceReservationCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                ServiceId = 1,
                ServiceDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateServiceReservation(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateServiceReservation - Retorna 409 cuando se alcanza la capacidad máxima")]
        public async Task CreateServiceReservation_Returns409_WhenCapacityReached()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Users.Add(new User { Id = 2, Name = "User 2" });
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 1,
                IsAvailable = true,
                Capacity = 1
            });

            var serviceDate = DateTime.UtcNow.Date.AddDays(1);
            context.ServiceReservations.Add(new ServiceReservation
            {
                Id = 1,
                UserId = 1,
                ShelterId = 1,
                ServiceId = 1,
                ServiceDate = serviceDate,
                IsActive = true
            });
            context.SaveChanges();

            var dto = new ServiceReservationCreateDto
            {
                UserId = 2,
                ShelterId = 1,
                ServiceId = 1,
                ServiceDate = serviceDate
            };

            var response = await manager.CreateServiceReservation(dto);

            Assert.Equal("409", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateServiceReservation - Retorna 200 cuando se crea correctamente")]
        public async Task CreateServiceReservation_Returns200_WhenCreatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            context.Users.Add(new User { Id = 1, Name = "User 1" });
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Services.Add(new Service { Id = 1, Name = "Comida" });
            context.ShelterServices.Add(new ShelterService
            {
                ShelterId = 1,
                ServiceId = 1,
                IsAvailable = true,
                Capacity = 10
            });
            context.SaveChanges();

            var dto = new ServiceReservationCreateDto
            {
                UserId = 1,
                ShelterId = 1,
                ServiceId = 1,
                ServiceDate = DateTime.UtcNow.Date.AddDays(1)
            };

            var response = await manager.CreateServiceReservation(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.UserId);
            Assert.Equal(1, response.Data.ShelterId);
            Assert.Equal(1, response.Data.ServiceId);
            Assert.True(response.Data.IsActive);
            Assert.NotEmpty(response.Data.QrData);
        }

        #endregion

        #region UpdateServiceReservationIsActive

        [Fact(DisplayName = "UpdateServiceReservationIsActive - Retorna 404 cuando la ServiceReservation no existe")]
        public async Task UpdateServiceReservationIsActive_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new ServiceReservationPatchIsActiveDto
            {
                Id = 999,
                IsActive = false
            };

            var response = await manager.UpdateServiceReservationIsActive(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateServiceReservationIsActive - Retorna 200 cuando se actualiza correctamente")]
        public async Task UpdateServiceReservationIsActive_Returns200_WhenUpdatedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var dto = new ServiceReservationPatchIsActiveDto
            {
                Id = 1,
                IsActive = false
            };

            var response = await manager.UpdateServiceReservationIsActive(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.False(response.Data.IsActive);
        }

        #endregion

        #region DeleteServiceReservation

        [Fact(DisplayName = "DeleteServiceReservation - Retorna 404 cuando la ServiceReservation no existe")]
        public async Task DeleteServiceReservation_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteServiceReservation(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteServiceReservation - Retorna 200 cuando se elimina correctamente")]
        public async Task DeleteServiceReservation_Returns200_WhenDeletedSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.DeleteServiceReservation(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(1, response.Data.Id);

            var deleted = await context.ServiceReservations.FindAsync(1);
            Assert.Null(deleted);
        }

        #endregion
    }
}