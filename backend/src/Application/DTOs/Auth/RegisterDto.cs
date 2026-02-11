using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public record RegisterDto(
    string Username,
    string Password,
    string FirstName,
    string LastName,
    string Dni,
    string PhoneNumber,
    string Email,
    string Role // "Admin" o "Employee"
);