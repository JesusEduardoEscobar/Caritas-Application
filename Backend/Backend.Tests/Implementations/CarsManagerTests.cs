using Backend.Dtos;
using Backend.Implementations;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Backend.Tests.Implementations
{
    public class CarsManagerTests
    {
        private CarsManager CreateManagerWithDb(out NeonTechDbContext context)
        {
            context = TestHelper.CreateInMemoryDbContext();
            var logger = new LoggerFactory().CreateLogger<CarsManager>();
            return new CarsManager(context, logger);
        }

        private void PopulateDBContext(ref NeonTechDbContext context)
        {
            // Agregar shelters
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });

            // Agregar carros
            context.Cars.Add(TestEntry());
            context.Cars.Add(new Car { Id = 2, ShelterId = 1, Plate = "ABC-456", Model = "Toyota Camry", Capacity = 5 });
            context.Cars.Add(new Car { Id = 3, ShelterId = 1, Plate = "ABC-789", Model = "Honda Civic", Capacity = 5 });
            context.Cars.Add(new Car { Id = 4, ShelterId = 2, Plate = "XYZ-123", Model = "Ford Focus", Capacity = 5 });
            context.Cars.Add(new Car { Id = 5, ShelterId = 2, Plate = "XYZ-456", Model = "Mazda 3", Capacity = 5 });

            context.SaveChanges();
        }

        private Car TestEntry()
        {
            return new Car
            {
                Id = 1,
                ShelterId = 1,
                Plate = "ABC-123",
                Model = "Toyota Corolla",
                Capacity = 5
            };
        }

        #region GET

        // ---------- GetCars ----------

        [Fact(DisplayName = "GetCars - Retorna lista vacía cuando no hay Carros")]
        public async Task GetCars_ReturnsEmpty_WhenNoCars()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetCars();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Equal(0, response.RowsCount);
        }

        [Fact(DisplayName = "GetCars - Retorna todos los carros sin filtros")]
        public async Task GetCars_ReturnsAll_WhenNoFilters()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetCars();

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(5, response.RowsCount);
        }

        [Fact(DisplayName = "GetCars - Retorna con filtro correctamente: ShelterId")]
        public async Task GetCars_ReturnsFiltered_ByShelterId()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var response = await manager.GetCars(shelterId: 1);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.RowsCount);
            Assert.All(response.Data, car => Assert.Equal(1, car.ShelterId));
        }

        [Fact(DisplayName = "GetCars - Retorna lista vacía cuando el ShelterId no tiene carros")]
        public async Task GetCars_ReturnsEmpty_WhenShelterHasNoCars()
        {
            var manager = CreateManagerWithDb(out var context);
            
            context.Shelters.Add(new Shelter { Id = 99, Name = "Empty Shelter" });
            await context.SaveChangesAsync();

            var response = await manager.GetCars(shelterId: 99);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Equal(0, response.RowsCount);
        }

        // ---------- GetCar ----------

        [Fact(DisplayName = "GetCar - Retorna 404 cuando el ID no existe")]
        public async Task GetCar_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.GetCar(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "GetCar - Retorna correctamente cuando el ID existe")]
        public async Task GetCar_ReturnsCar_WhenExists()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var car = TestEntry();

            var response = await manager.GetCar(car.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(car.Id, response.Data.Id);
            Assert.Equal(car.ShelterId, response.Data.ShelterId);
            Assert.Equal(car.Plate, response.Data.Plate);
            Assert.Equal(car.Model, response.Data.Model);
            Assert.Equal(car.Capacity, response.Data.Capacity);
        }

        #endregion

        #region POST

        [Fact(DisplayName = "CreateCar - Retorna 400 si el DTO es null")]
        public async Task CreateCar_Returns400_WhenDtoIsNull()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.CreateCar(null!);

            Assert.Equal("400", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateCar - Retorna 404 si el Shelter no existe")]
        public async Task CreateCar_Returns404_WhenShelterNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var dto = new CarCreateDto
            {
                ShelterId = 999,
                Plate = "ABC-123",
                Model = "Toyota Corolla",
                Capacity = 5
            };

            var response = await manager.CreateCar(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "CreateCar - Crea correctamente un nuevo carro")]
        public async Task CreateCar_CreatesSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);

            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            await context.SaveChangesAsync();

            var dto = new CarCreateDto
            {
                ShelterId = 1,
                Plate = "ABC-123",
                Model = "Toyota Corolla",
                Capacity = 5
            };

            var response = await manager.CreateCar(dto);

            Assert.Equal("201", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(dto.ShelterId, response.Data.ShelterId);
            Assert.Equal(dto.Plate, response.Data.Plate);
            Assert.Equal(dto.Model, response.Data.Model);
            Assert.Equal(dto.Capacity, response.Data.Capacity);

            Assert.Equal(1, context.Cars.Count());
        }

        #endregion

        #region PUT

        [Fact(DisplayName = "UpdateCar - Retorna 404 si el Shelter no existe")]
        public async Task UpdateCar_Returns404_WhenShelterNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var car = TestEntry();
            context.Cars.Add(car);
            await context.SaveChangesAsync();

            var dto = new CarPutDto
            {
                Id = car.Id,
                ShelterId = 999,
                Plate = "XYZ-999",
                Model = "Updated Model",
                Capacity = 7
            };

            var response = await manager.UpdateCar(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateCar - Retorna 404 si el Carro no existe")]
        public async Task UpdateCar_Returns404_WhenCarNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            await context.SaveChangesAsync();

            var dto = new CarPutDto
            {
                Id = 999,
                ShelterId = 1,
                Plate = "XYZ-999",
                Model = "Updated Model",
                Capacity = 7
            };

            var response = await manager.UpdateCar(dto);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "UpdateCar - Actualiza correctamente un carro existente")]
        public async Task UpdateCar_UpdatesSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);

            var car = TestEntry();
            context.Shelters.Add(new Shelter { Id = 1, Name = "Shelter 1" });
            context.Shelters.Add(new Shelter { Id = 2, Name = "Shelter 2" });
            context.Cars.Add(car);
            await context.SaveChangesAsync();

            var dto = new CarPutDto
            {
                Id = car.Id,
                ShelterId = 2,
                Plate = "XYZ-999",
                Model = "Updated Model",
                Capacity = 7
            };

            var response = await manager.UpdateCar(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(dto.Id, response.Data.Id);
            Assert.Equal(dto.ShelterId, response.Data.ShelterId);
            Assert.Equal(dto.Plate, response.Data.Plate);
            Assert.Equal(dto.Model, response.Data.Model);
            Assert.Equal(dto.Capacity, response.Data.Capacity);

            Assert.Equal(1, context.Cars.Count());
        }

        [Fact(DisplayName = "UpdateCar - Actualiza todos los campos correctamente")]
        public async Task UpdateCar_UpdatesAllFields()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var originalCar = await context.Cars.FindAsync(1);
            Assert.NotNull(originalCar);

            var dto = new CarPutDto
            {
                Id = 1,
                ShelterId = 2,
                Plate = "NEW-PLATE",
                Model = "Brand New Model",
                Capacity = 10
            };

            var response = await manager.UpdateCar(dto);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            
            // Verificar que todos los campos se actualizaron
            Assert.NotEqual(originalCar.ShelterId, response.Data.ShelterId);
            Assert.NotEqual(originalCar.Plate, response.Data.Plate);
            Assert.NotEqual(originalCar.Model, response.Data.Model);
            Assert.NotEqual(originalCar.Capacity, response.Data.Capacity);
        }

        #endregion

        #region DELETE

        [Fact(DisplayName = "DeleteCar - Retorna 404 si el Carro no existe")]
        public async Task DeleteCar_Returns404_WhenNotFound()
        {
            var manager = CreateManagerWithDb(out var context);

            var response = await manager.DeleteCar(999);

            Assert.Equal("404", response.Code);
            Assert.Null(response.Data);
        }

        [Fact(DisplayName = "DeleteCar - Elimina correctamente un carro existente")]
        public async Task DeleteCar_DeletesSuccessfully()
        {
            var manager = CreateManagerWithDb(out var context);

            var car = TestEntry();
            context.Cars.Add(car);
            await context.SaveChangesAsync();

            var response = await manager.DeleteCar(car.Id);

            Assert.Equal("200", response.Code);
            Assert.NotNull(response.Data);
            Assert.Equal(car.Id, response.Data.Id);

            var deleted = await context.Cars.FindAsync(car.Id);
            Assert.Null(deleted);
        }

        [Fact(DisplayName = "DeleteCar - El conteo de carros disminuye después de eliminar")]
        public async Task DeleteCar_DecreasesCount()
        {
            var manager = CreateManagerWithDb(out var context);
            PopulateDBContext(ref context);

            var initialCount = context.Cars.Count();
            Assert.Equal(5, initialCount);

            await manager.DeleteCar(1);

            var finalCount = context.Cars.Count();
            Assert.Equal(4, finalCount);
        }

        #endregion
    }
}