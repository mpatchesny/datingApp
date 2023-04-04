using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class ChangeUserSettingsHandler : ICommandHandler<ChangeUserSettings>
{
    private readonly IUserRepository _userRepository;
    public ChangeUserSettingsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(ChangeUserSettings command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        user.Settings.ChangeDiscoverAge(command.DiscoverAgeFrom, command.DiscoverAgeTo);
        user.Settings.ChangeDiscoverRange(command.DiscoverRange);
        user.Settings.ChangeDiscoverSex((Sex) command.DiscoverSex);
    }
}