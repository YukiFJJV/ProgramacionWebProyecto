using MediatR;
using PsyLink.Application.Features.Users.DTOs;

namespace PsyLink.Application.Features.Users.Services.Commands;

public record CreateUserCommand(UserCreateRequestDto User) : ICommand<UserResponseDto>;
