using System.ComponentModel.DataAnnotations;

namespace Library.ControllerApi.DTOs;

public record RegisterDto(
    [Required, MaxLength(64)] string Username,
    [Required, MaxLength(8)] string Password
);


public record LoginDto(
    [Required] string Username,
    [Required] string Password
);