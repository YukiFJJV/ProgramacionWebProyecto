using MediatR;
using PsyLink.Application.Features.Specialties.DTOs;

namespace PsyLink.Application.Features.Specialties.Services.Commands;

public record CreateSpecialtyCommand(string SpecialtyName) : IRequest<SpecialtyResponseDto>;
