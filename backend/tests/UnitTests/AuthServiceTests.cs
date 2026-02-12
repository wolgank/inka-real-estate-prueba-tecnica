using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using Moq;
using Xunit;
using BCrypt.Net;

namespace UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly RegisterValidator _registerValidator;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _registerValidator = new RegisterValidator();
        
        _authService = new AuthService(
            _userRepositoryMock.Object, 
            _jwtProviderMock.Object, 
            _registerValidator);
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenDniIsInvalid()
    {
        // Arrange: DNI de 5 dígitos (inválido según nuestras reglas)
        var dto = new RegisterDto("user test", "pass123", "Daniel", "Guti", "123", "999", "test@test.com", "Employee");

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _authService.RegisterAsync(dto));
    }

    [Fact]
    public async Task RegisterAsync_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var dto = new RegisterDto("daniguti", "password123", "Daniel", "Gutierrez", "70809010", "987654321", "test@inka.com", "Admin");
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(dto.Username))
            .ReturnsAsync((User?)null); // El usuario no existe previamente

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Username, result.Username);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        // Arrange
        var password = "password123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Username = "daniel", PasswordHash = passwordHash, Role = Role.Admin };
        
        var loginDto = new LoginDto 
        { 
            Username = "daniel", 
            Password = password 
        };
        
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync("daniel"))
            .ReturnsAsync(user);
        _jwtProviderMock.Setup(x => x.Create(user))
            .Returns("fake-jwt-token");

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.Equal("fake-jwt-token", result.Token);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new User { Username = "daniel", PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_pass") };
        var loginDto = new LoginDto 
        { 
            Username = "daniel", 
            Password = "wrong_pass"
        };
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync("daniel"))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginDto));
        Assert.Equal("Credenciales inválidas", exception.Message);
    }
}