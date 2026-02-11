using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public class RegisterDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;
}