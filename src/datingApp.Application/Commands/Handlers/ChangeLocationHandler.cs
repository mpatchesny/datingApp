using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class ChangeLocationHandler : ICommandHandler<ChangeLocation>
{
    private readonly IUserRepository _userRepository;
    public ChangeLocationHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(ChangeLocation command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }
        user.Settings.ChangeLocation(command.Lat, command.Lon);
        await _userRepository.UpdateAsync(user);
    }
}