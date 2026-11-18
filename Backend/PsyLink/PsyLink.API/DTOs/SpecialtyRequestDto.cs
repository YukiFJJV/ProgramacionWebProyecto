using System.ComponentModel.DataAnnotations;

namespace PsyLink.API.DTOs;

/// <summary>Specialty name to create or update.</summary>
public sealed record SpecialtyRequestDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public required string SpecialtyName { get; init; }
}
