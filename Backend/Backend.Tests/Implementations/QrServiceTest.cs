using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class QrServiceTests
    {
        private QrService CreateServiceWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<QrService>();
            return new QrService(context, logger);
        }

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            context.Users.Add(new User { Id = 1, Name = "Test User 1" });
            context.Users.Add(new User { Id = 2, Name = "Test User 2" });

            context.Reservations.Add(new Reservation { Id = 1, UserId = 1 });
            context.Reservations.Add(new Reservation { Id = 2, UserId = 2 });

            context.ServiceReservations.Add(new ServiceReservation { Id = 1, UserId = 1 });
            context.ServiceReservations.Add(new ServiceReservation { Id = 2, UserId = 2 });

            context.TransportRequests.Add(new TransportRequest { Id = 1, UserId = 1 });
            context.TransportRequests.Add(new TransportRequest { Id = 2, UserId = 2 });

            context.SaveChanges();
        }

        #region ReadQr

        [Fact(DisplayName = "ReadQr - Retorna 400 cuando el formato del QR es inválido")]
        public async Task ReadQr_Returns400_WhenFormatIsInvalid()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.ReadQr("InvalidQrFormat");

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 400 cuando el tipo no es válido")]
        public async Task ReadQr_Returns400_WhenTypeIsInvalid()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.ReadQr("InvalidType-1-User-1");

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando la Reservación no existe")]
        public async Task ReadQr_Returns404_WhenReservationNotFound()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.ReadQr("Reservation-999-User-1");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando el Usuario de la Reservación no existe")]
        public async Task ReadQr_Returns404_WhenReservationUserNotFound()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("Reservation-1-User-999");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando el UserId no coincide con la Reservación")]
        public async Task ReadQr_Returns404_WhenReservationUserIdMismatch()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("Reservation-1-User-2");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Lee correctamente un QR de Reservación")]
        public async Task ReadQr_ReadsReservationQr_Successfully()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("Reservation-1-User-1");

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("Reservation", response.Data.Type);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal(1, response.Data.UserId);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando la Reservación de Servicio no existe")]
        public async Task ReadQr_Returns404_WhenServiceReservationNotFound()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.ReadQr("ServiceReservation-999-User-1");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando el Usuario de la Reservación de Servicio no existe")]
        public async Task ReadQr_Returns404_WhenServiceReservationUserNotFound()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("ServiceReservation-1-User-999");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando el UserId no coincide con la Reservación de Servicio")]
        public async Task ReadQr_Returns404_WhenServiceReservationUserIdMismatch()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("ServiceReservation-1-User-2");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Lee correctamente un QR de Reservación de Servicio")]
        public async Task ReadQr_ReadsServiceReservationQr_Successfully()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("ServiceReservation-1-User-1");

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("ServiceReservation", response.Data.Type);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal(1, response.Data.UserId);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando la Petición de Transporte no existe")]
        public async Task ReadQr_Returns404_WhenTransportRequestNotFound()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.ReadQr("TransportRequest-999-User-1");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando el Usuario de la Petición de Transporte no existe")]
        public async Task ReadQr_Returns404_WhenTransportRequestUserNotFound()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("TransportRequest-1-User-999");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Retorna 404 cuando el UserId no coincide con la Petición de Transporte")]
        public async Task ReadQr_Returns404_WhenTransportRequestUserIdMismatch()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("TransportRequest-1-User-2");

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "ReadQr - Lee correctamente un QR de Petición de Transporte")]
        public async Task ReadQr_ReadsTransportRequestQr_Successfully()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.ReadQr("TransportRequest-1-User-1");

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("TransportRequest", response.Data.Type);
            Assert.Equal(1, response.Data.Id);
            Assert.Equal(1, response.Data.UserId);
        }

        #endregion

        #region GenerateReservationQr

        [Fact(DisplayName = "GenerateReservationQr - Retorna 404 cuando la Reservación no existe")]
        public async Task GenerateReservationQr_Returns404_WhenReservationNotFound()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.GenerateReservationQr(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GenerateReservationQr - Retorna 404 cuando el Usuario no existe")]
        public async Task GenerateReservationQr_Returns404_WhenUserNotFound()
        {
            var service = CreateServiceWithDb(out var context);
            context.Reservations.Add(new Reservation { Id = 1, UserId = 999 });
            await context.SaveChangesAsync();

            var response = await service.GenerateReservationQr(1);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GenerateReservationQr - Genera QR correctamente")]
        public async Task GenerateReservationQr_GeneratesQr_Successfully()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.GenerateReservationQr(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("Reservation-1-User-1", response.Data);
        }

        #endregion

        #region GenerateServiceReservationQr

        [Fact(DisplayName = "GenerateServiceReservationQr - Retorna 404 cuando la Reservación de Servicio no existe")]
        public async Task GenerateServiceReservationQr_Returns404_WhenServiceReservationNotFound()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.GenerateServiceReservationQr(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GenerateServiceReservationQr - Retorna 404 cuando el Usuario no existe")]
        public async Task GenerateServiceReservationQr_Returns404_WhenUserNotFound()
        {
            var service = CreateServiceWithDb(out var context);
            context.ServiceReservations.Add(new ServiceReservation { Id = 1, UserId = 999 });
            await context.SaveChangesAsync();

            var response = await service.GenerateServiceReservationQr(1);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GenerateServiceReservationQr - Genera QR correctamente")]
        public async Task GenerateServiceReservationQr_GeneratesQr_Successfully()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.GenerateServiceReservationQr(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("ServiceReservation-1-User-1", response.Data);
        }

        #endregion

        #region GenerateTransportRequestQr

        [Fact(DisplayName = "GenerateTransportRequestQr - Retorna 404 cuando la Petición de Transporte no existe")]
        public async Task GenerateTransportRequestQr_Returns404_WhenTransportRequestNotFound()
        {
            var service = CreateServiceWithDb(out var context);

            var response = await service.GenerateTransportRequestQr(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GenerateTransportRequestQr - Retorna 404 cuando el Usuario no existe")]
        public async Task GenerateTransportRequestQr_Returns404_WhenUserNotFound()
        {
            var service = CreateServiceWithDb(out var context);
            context.TransportRequests.Add(new TransportRequest { Id = 1, UserId = 999 });
            await context.SaveChangesAsync();

            var response = await service.GenerateTransportRequestQr(1);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GenerateTransportRequestQr - Genera QR correctamente")]
        public async Task GenerateTransportRequestQr_GeneratesQr_Successfully()
        {
            var service = CreateServiceWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await service.GenerateTransportRequestQr(1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal("TransportRequest-1-User-1", response.Data);
        }

        #endregion
    }
}