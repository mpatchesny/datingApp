using datingApp.Application.Abstractions;

namespace datingApp.Application.Commands;

public sealed record RefreshJWT(string RefreshToken) : ICommand;