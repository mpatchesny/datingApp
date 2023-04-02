using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public class SingUpHandler : ICommandHandler<SingUp>
{
    private readonly IUserRepository _userRepository;
    public SingUpHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task HandleAsync(SingUp command)
    {
        var existingUser = await _userRepository.GetByEmailAsync(command.email);
        if (existingUser != null)
        {
            throw new EmailAlreadyInUseException(command.email);
        }
        
        existingUser = await _userRepository.GetByPhoneAsync(command.phone);
        if (existingUser != null)
        {
            throw new PhoneAlreadyInUseException(command.phone);
        }

        var settings = new UserSettings(0, (Sex) command.discoverSex, 18, 35, 30, 0.0, 0.0);
        var user = new User(0, command.phone, command.email, command.name, command.dateOfBirth, (Sex) command.sex, null, settings, command.job, command.bio);

        await _userRepository.AddAsync(user);
    }
}