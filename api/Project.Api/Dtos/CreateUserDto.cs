using System.ComponentModel.DataAnnotations;

namespace Project.Api.Dtos;
public record CreateUserDto(
    [Required] [EmailAddress] string email,
    [Required] string pass
);
