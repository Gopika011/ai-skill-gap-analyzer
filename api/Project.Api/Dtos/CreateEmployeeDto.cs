using System.ComponentModel.DataAnnotations;

namespace Project.Api.Dtos;

public record CreateEmployeeDto(
    [Required] string Id,
    [Required] string Name,
    [Required] [EmailAddress] string Email
);
