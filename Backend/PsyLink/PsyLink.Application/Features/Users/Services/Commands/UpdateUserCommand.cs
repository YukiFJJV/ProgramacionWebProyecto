using MediatR;
using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Users.Services.Commands;

public record UpdateUserCommand(
    Guid UserId,
    string? Email,
    bool EmailSpecified,
    string? FirstName,
    bool FirstNameSpecified,
    string? LastName,
    bool LastNameSpecified,
    string? PhoneNumber,
    bool PhoneNumberSpecified,
    Guid? RoleId,
    bool RoleIdSpecified,
    bool? Active,
    bool ActiveSpecified) : ICommand<UserResponseDto>;
