// using Backend.Implementations;
// using Backend.Infraestructure.Database;
// using Backend.Infraestructure.Models;
// using Backend.Tests.TestHelpers;
// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Threading.Tasks;
// using Xunit;

// namespace Backend.Tests.Implementations
// {
//     public class AuthenticatorTests : IDisposable
//     {
//         private readonly NeonTechDbContext _context;
//         private readonly Authenticator _authenticator;
//         private readonly string _testPassword = "StrongPassword123!";
//         private readonly string _testPasswordHash;

//         public AuthenticatorTests()
//         {
//             _context = TestHelper.CreateInMemoryDbContext();
//             _authenticator = new Authenticator(_context);
//             _testPasswordHash = BCrypt.Net.BCrypt.HashPassword(_testPassword);
//         }

//         public void Dispose()
//         {
//             _context.Database.EnsureDeleted();
//             _context.Dispose();
//         }

//         private async Task<User> SeedUser(string email, UserRole role = UserRole.user, bool verified = true, int? shelterId = null)
//         {
//             var user = new User
//             {
//                 Name = "Test User",
//                 Email = email,
//                 Password = _testPasswordHash,
//                 Role = role,
//                 Verified = verified,
//                 ShelterId = shelterId,
//                 Phone = "1234567890",
//                 DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
//                 EconomicLevel = EconomicLevel.medium
//             };
//             _context.Users.Add(user);
//             await _context.SaveChangesAsync();
//             return user;
//         }

//         // --- Pruebas para Login ---

//         [Fact]
//         public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
//         {
//             // Arrange
//             await SeedUser("test@example.com");

//             // Act
//             var result = await _authenticator.Login("test@example.com", _testPassword);

//             // Assert
//             Assert.Equal("200", result.StatusCode);
//             Assert.Equal("Autenticación exitosa", result.Message);
//             Assert.NotNull(result.Response);
//             Assert.NotNull(result.Response.token);
//         }

//         [Fact]
//         public async Task Login_WithInvalidEmail_ReturnsFault401()
//         {
//             // Arrange
//             await SeedUser("test@example.com");

//             // Act
//             var result = await _authenticator.Login("wrong@example.com", _testPassword);

//             // Assert
//             Assert.Equal("401", result.StatusCode);
//             Assert.Equal("Credenciales inválidas", result.Message);
//         }

//         [Fact]
//         public async Task Login_WithInvalidPassword_ReturnsFault401()
//         {
//             // Arrange
//             await SeedUser("test@example.com");

//             // Act
//             var result = await _authenticator.Login("test@example.com", "WrongPassword");

//             // Assert
//             Assert.Equal("401", result.StatusCode);
//             Assert.Equal("Formato de la constraseña incorecto", result.Message);
//         }

//         // --- Pruebas para RegisterLite ---

//         [Fact]
//         public async Task RegisterLite_WithValidData_ReturnsSuccessAndUser()
//         {
//             // Arrange
//             var dob = new DateTime(1995, 5, 5, 0, 0, 0, DateTimeKind.Utc);

//             // Act
//             var result = await _authenticator.RegisterLite("Lite User", "lite@example.com", _testPassword, "+12345678901", dob);
//             var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "lite@example.com");

//             // Assert
//             Assert.Equal("201", result.StatusCode);
//             Assert.NotNull(userInDb);
//             Assert.Equal("Lite User", userInDb.Name);
//             Assert.Equal(UserRole.user, userInDb.Role);
//             Assert.False(userInDb.Verified);
//         }

//         [Fact]
//         public async Task RegisterLite_WithExistingEmail_ReturnsFault409()
//         {
//             // Arrange
//             await SeedUser("lite@example.com");
//             var dob = new DateTime(1995, 5, 5, 0, 0, 0, DateTimeKind.Utc);

//             // Act
//             var result = await _authenticator.RegisterLite("Another User", "lite@example.com", _testPassword, "+12345678901", dob);

//             // Assert
//             Assert.Equal("409", result.StatusCode);
//             Assert.Equal("El correo ya está en uso", result.Message);
//         }

//         [Fact]
//         public async Task RegisterLite_WithFutureDateOfBirth_ReturnsFault400()
//         {
//             // Arrange
//             var futureDob = DateTime.Today.AddDays(1);

//             // Act
//             var result = await _authenticator.RegisterLite("Future User", "future@example.com", _testPassword, "+12345678901", futureDob);

//             // Assert
//             Assert.Equal("400", result.StatusCode);
//             Assert.Equal("La fecha de nacimiento no puede ser futura", result.Message);
//         }

//         [Fact]
//         public async Task RegisterLite_WithInvalidPhone_ReturnsFault400()
//         {
//             // Arrange
//             var dob = new DateTime(1995, 5, 5, 0, 0, 0, DateTimeKind.Utc);

//             // Act
//             var result = await _authenticator.RegisterLite("Lite User", "lite@example.com", _testPassword, "123", dob);

//             // Assert
//             Assert.Equal("400", result.StatusCode);
//             Assert.Equal("El número de teléfono debe tener 10 dígitos", result.Message);
//         }

//         // --- Pruebas para RegisterUser (Actualización) ---

//         [Fact]
//         public async Task RegisterUser_WithValidEmail_UpdatesUser()
//         {
//             // Arrange
//             var user = await SeedUser("update@example.com", UserRole.user, false, null);
//             Assert.False(user.Verified);
//             Assert.Null(user.ShelterId);

//             // Act
//             var result = await _authenticator.RegisterUser("update@example.com", "0987654321", 10, "High", true);
//             var updatedUser = await _context.Users.FindAsync(user.Id);

//             // Assert
//             Assert.Equal("200", result.StatusCode);
//             Assert.True(updatedUser.Verified);
//             Assert.Equal(10, updatedUser.ShelterId);
//             Assert.Equal(EconomicLevel.High, updatedUser.EconomicLevel);
//         }

//         [Fact]
//         public async Task RegisterUser_WithNonExistentEmail_ReturnsFault404()
//         {
//             // Act
//             var result = await _authenticator.RegisterUser("notfound@example.com", "0987654321", 10, "High", true);

//             // Assert
//             Assert.Equal("404", result.StatusCode);
//             Assert.Equal("Usuario no encontrado con ese correo", result.Message);
//         }

//         [Fact]
//         public async Task RegisterUser_WithExistingShelterId_DoesNotOverwriteShelterId()
//         {
//             // Arrange
//             var user = await SeedUser("update@example.com", UserRole.user, false, 5); // ShelterId inicial es 5

//             // Act
//             // Intenta "registrar" (actualizar) con ShelterId 10
//             var result = await _authenticator.RegisterUser("update@example.com", "0987654321", 10, "High", true);
//             var updatedUser = await _context.Users.FindAsync(user.Id);

//             // Assert
//             // El ShelterId no debe cambiar porque ya tenía uno (5)
//             Assert.Equal("200", result.StatusCode);
//             Assert.Equal(5, updatedUser.ShelterId);
//         }

//         // --- Pruebas para CreateUser ---

//         [Fact]
//         public async Task CreateUser_WithValidData_ReturnsSuccess()
//         {
//             // Arrange
//             var dob = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

//             // Act
//             var result = await _authenticator.CreateUser("New User", "new@example.com", _testPassword, _testPassword, "521234567890", dob, 1, "Low", true);
//             var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "new@example.com");

//             // Assert
//             Assert.Equal("201", result.StatusCode);
//             Assert.NotNull(userInDb);
//             Assert.Equal("New User", userInDb.Name);
//             Assert.Equal(1, userInDb.ShelterId);
//             Assert.True(userInDb.Verified);
//             Assert.Equal(EconomicLevel.Low, userInDb.EconomicLevel);
//         }

//         [Fact]
//         public async Task CreateUser_WithMismatchedPasswords_ReturnsFault400()
//         {
//             // Arrange
//             var dob = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

//             // Act
//             var result = await _authenticator.CreateUser("New User", "new@example.com", _testPassword, "WrongConfirm", "521234567890", dob, 1, "Low", true);

//             // Assert
//             Assert.Equal("400", result.StatusCode);
//             Assert.Equal("Las contraseñas no coinciden", result.Message);
//         }

//         [Fact]
//         public async Task CreateUser_WithInvalidPhoneFormat_ReturnsFault400()
//         {
//             // Arrange
//             var dob = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

//             // Act
//             var result = await _authenticator.CreateUser("New User", "new@example.com", _testPassword, _testPassword, "123", dob, 1, "Low", true);

//             // Assert
//             Assert.Equal("400", result.StatusCode);
//             Assert.Equal("El número de teléfono debe tener 10 dígitos", result.Message);
//         }

//         // --- Pruebas para RegisterAdmin ---

//         [Fact]
//         public async Task RegisterAdmin_WithValidCredentials_ReturnsSuccess()
//         {
//             // Arrange
//             await SeedUser("admin@example.com", UserRole.admin);

//             // Act
//             var result = await _authenticator.RegisterAdmin("New Admin", "newadmin@example.com", "newPass123", "admin@example.com", _testPassword);
//             var newAdminInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "newadmin@example.com");

//             // Assert
//             Assert.Equal("201", result.StatusCode);
//             Assert.NotNull(newAdminInDb);
//             Assert.Equal("New Admin", newAdminInDb.Name);
//             Assert.Equal(UserRole.admin, newAdminInDb.Role);
//         }

//         [Fact]
//         public async Task RegisterAdmin_WithInvalidAdminCredentials_ReturnsFault403()
//         {
//             // Arrange
//             await SeedUser("admin@example.com", UserRole.admin);

//             // Act
//             var result = await _authenticator.RegisterAdmin("New Admin", "newadmin@example.com", "newPass123", "admin@example.com", "WrongAdminPassword");

//             // Assert
//             Assert.Equal("403", result.StatusCode);
//             Assert.Equal("Credenciales de administrador inválidas", result.Message);
//         }

//         [Fact]
//         public async Task RegisterAdmin_WithNonAdminCredentials_ReturnsFault403()
//         {
//             // Arrange
//             // El usuario "admin" es un usuario normal, no un admin
//             await SeedUser("notadmin@example.com", UserRole.user);

//             // Act
//             var result = await _authenticator.RegisterAdmin("New Admin", "newadmin@example.com", "newPass123", "notadmin@example.com", _testPassword);

//             // Assert
//             Assert.Equal("403", result.StatusCode);
//             Assert.Equal("Credenciales de administrador inválidas", result.Message);
//         }

//         // --- Pruebas para VerifyUser ---

//         [Fact]
//         public async Task VerifyUser_WithExistingUser_UpdatesVerification()
//         {
//             // Arrange
//             var user = await SeedUser("verify@example.com", UserRole.user, false);
//             Assert.False(user.Verified);

//             // Act
//             var result = await _authenticator.VerifyUser(user.Id, true);
//             var updatedUser = await _context.Users.FindAsync(user.Id);

//             // Assert
//             Assert.Equal("200", result.StatusCode);
//             Assert.True(updatedUser.Verified);
//         }

//         [Fact]
//         public async Task VerifyUser_WithNonExistentUser_ReturnsFault404()
//         {
//             // Act
//             var result = await _authenticator.VerifyUser(999, true);

//             // Assert
//             Assert.Equal("404", result.StatusCode);
//             Assert.Equal("Usuario no encontrado", result.Message);
//         }

//         // --- Pruebas para DeleteUser ---

//         [Fact]
//         public async Task DeleteUser_WithExistingUser_ReturnsSuccessAndRemovesUser()
//         {
//             // Arrange
//             var user = await SeedUser("delete@example.com");
//             var userId = user.Id;

//             // Act
//             var result = await _authenticator.DeleteUser(userId);
//             var deletedUser = await _context.Users.FindAsync(userId);

//             // Assert
//             Assert.Equal("200", result.StatusCode);
//             Assert.Null(deletedUser);
//         }

//         [Fact]
//         public async Task DeleteUser_WithNonExistentUser_ReturnsFault404()
//         {
//             // Act
//             var result = await _authenticator.DeleteUser(999);

//             // Assert
//             Assert.Equal("404", result.StatusCode);
//             Assert.Equal("Usuario no encontrado", result.Message);
//         }

//         // --- Pruebas para EditUser ---

//         [Fact]
//         public async Task EditUser_WithExistingUser_UpdatesFields()
//         {
//             // Arrange
//             var user = await SeedUser("edit@example.com", UserRole.user, false, null);

//             // Act
//             var result = await _authenticator.EditUser(user.Id, "Edited Name", "1112223333", 2, true, "bajo");
//             var updatedUser = await _context.Users.FindAsync(user.Id);

//             // Assert
//             Assert.Equal("200", result.StatusCode);
//             Assert.Equal("Edited Name", updatedUser.Name);
//             Assert.Equal("1112223333", updatedUser.Phone);
//             Assert.Equal(2, updatedUser.ShelterId);
//             Assert.True(updatedUser.Verified);
//             Assert.Equal(EconomicLevel.Low, updatedUser.EconomicLevel);
//         }

//         [Fact]
//         public async Task EditUser_WithNonExistentUser_ReturnsFault404()
//         {
//             // Act
//             var result = await _authenticator.EditUser(999, "Edited Name", "1112223333", 2, true, "bajo");

//             // Assert
//             Assert.Equal("404", result.StatusCode);
//             Assert.Equal("Usuario no encontrado", result.Message);
//         }

//         [Fact]
//         public async Task EditUser_WithInvalidEconomicLevel_ReturnsFault400()
//         {
//             // Arrange
//             var user = await SeedUser("edit@example.com");

//             // Act
//             var result = await _authenticator.EditUser(user.Id, null, null, null, null, "NivelInvalido");

//             // Assert
//             Assert.Equal("400", result.StatusCode);
//             Assert.Equal("Nivel económico inválido", result.Message);
//         }
//     }
// }