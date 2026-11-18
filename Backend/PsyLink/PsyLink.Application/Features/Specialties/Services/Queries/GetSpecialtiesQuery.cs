using MediatR;
using PsyLink.Application.Features.Specialties.DTOs;

namespace PsyLink.Application.Features.Specialties.Services.Queries;

public record GetSpecialtiesQuery : IQuery<IReadOnlyList<SpecialtyResponseDto>>;
