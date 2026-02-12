using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
namespace Application.Services;
using FluentValidation;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IValidator<RegisterDto> _validator;

    public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider,IValidator<RegisterDto> validator)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _validator = validator;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Validar
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        // Verificar existencia
        var existing = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existing != null) throw new Exception("El nombre de usuario ya existe.");

        // Mapeo y creación
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = Enum.TryParse<Role>(dto.Role, out var assignedRole) ? assignedRole : Role.Employee,
            Profile = new UserProfile
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Dni = dto.Dni,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email
            }
        };

        try 
        {
            await _userRepository.AddAsync(user);
        }
        catch (Exception ex)
        {
            // Si algo falla a nivel de DB, el middleware devolverá un error genérico 500
            // y la transacción de EF hará rollback automático.
            throw new Exception("Error al procesar el registro en la base de datos", ex);
        }

        return new RegisterResponseDto { Username = user.Username, Role = user.Role.ToString() };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new Exception("Credenciales inválidas");
        }

        return new AuthResponseDto
        {
            Username = user.Username,
            Role = user.Role.ToString(),
            Token = _jwtProvider.Create(user)
        };
    }
}