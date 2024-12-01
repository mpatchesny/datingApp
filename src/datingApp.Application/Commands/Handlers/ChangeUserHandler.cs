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
using datingApp.Core.ValueObjects;

namespace datingApp.Application.Commands.Handlers;

public sealed class ChangeUserHandler : ICommandHandler<ChangeUser>
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
            if (!DateOnly.TryParseExact(command.DateOfBirth, new string[] { "yyyy-MM-dd" }, out DateOnly newDateOfBirth))
            {
                throw new InvalidDateOfBirthFormatException(command.DateOfBirth);
            }
            user.ChangeDateOfBirth(newDateOfBirth);
        }

        if (command.PreferredAgeFrom != null && command.PreferredAgeTo != null)
        {
            var newPreferredAge = new PreferredAge((int) command.PreferredAgeFrom,(int) command.PreferredAgeTo);
            user.Settings.ChangePreferredAge(newPreferredAge);
        }
        if (command.PreferredRange != null) 
        {
            user.Settings.ChangePreferredMaxDistance((int) command.PreferredRange);
        }
        if (command.PreferredSex != null) 
        {
            user.Settings.ChangePreferredSex((PreferredSex) command.PreferredSex);
        }
        if (command.Lat != null && command.Lon != null)
        {
            var newLocation = new Location((double) command.Lat, (double) command.Lon);
            user.Settings.ChangeLocation(newLocation);
        }

        await _userRepository.UpdateAsync(user);
    }
}