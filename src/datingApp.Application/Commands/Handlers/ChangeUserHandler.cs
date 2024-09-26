using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Security;
using datingApp.Core.Consts;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public class ChangeUserHandler : ICommandHandler<ChangeUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public ChangeUserHandler(IUserRepository userRepository, IDatingAppAuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(ChangeUser command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, user, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        if (command.Bio != null) user.ChangeBio(command.Bio);
        if (command.Job != null) user.ChangeJob(command.Job);
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
            user.Settings.ChangePreferredAge((int) command.DiscoverAgeFrom, (int) command.DiscoverAgeTo);
        }
        if (command.DiscoverRange != null) 
        {
            user.Settings.ChangePreferredMaxDistance((int) command.DiscoverRange);
        }
        if (command.DiscoverSex != null) 
        {
            user.Settings.ChangePreferredSex((PreferredSex) command.DiscoverSex);
        }
        if (command.Lat != null && command.Lon != null)
        {
            user.Settings.ChangeLocation((double) command.Lat, (double) command.Lon);
        }

        await _userRepository.UpdateAsync(user);
    }
}