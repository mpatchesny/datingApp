using datingApp.Application.Abstractions;

namespace datingApp.Application.Commands;

public sealed record RefreshToken(string Token) : AuthenticatedCommandBase;