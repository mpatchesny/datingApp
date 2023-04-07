using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public class ChangeUserHandler : ICommandHandler<ChangeUser>
{
    private readonly IUserRepository _userRepository;
    public ChangeUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(ChangeUser command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        if (command.Bio == null && command.Job == null && command.DateOfBirth == null)
        {
            return;
        }

        if (command.Bio != null) user.ChangeBio(command.Bio);
        if (command.Job != null) user.ChangeBio(command.Job);
        if (command.DateOfBirth != null)
        {
            if (!DateOnly.TryParseExact(command.DateOfBirth, new string[] { "yyyy-MM-dd" }, out DateOnly dob))
            {
                throw new InvalidDateOfBirthFormatException(command.DateOfBirth);
            }
            user.ChangeDateOfBirth(dob);
        }
        if (command.DiscoverAgeFrom != null && command.DiscoverAgeTo != null)
        {
            user.Settings.ChangeDiscoverAge((int) command.DiscoverAgeFrom, (int) command.DiscoverAgeTo);
        }
        if (command.DiscoverRange != null) user.Settings.ChangeDiscoverRange((int) command.DiscoverRange);
        if (command.DiscoverSex != null) user.Settings.ChangeDiscoverSex((Sex) command.DiscoverSex);

        await _userRepository.UpdateAsync(user);
    }
}