using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class SignUpHandler : ICommandHandler<SignUp>
{
    private readonly IUserRepository _userRepository;
    public SignUpHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task HandleAsync(SignUp command)
    {
        if (!DateOnly.TryParseExact(command.DateOfBirth, new string[] { "yyyy-MM-dd" }, out DateOnly dob))
        {
            throw new InvalidDateOfBirthFormatException(command.DateOfBirth);
        }

        var existingUser = await _userRepository.GetByEmailAsync(command.Email);
        if (existingUser != null)
        {
            throw new EmailAlreadyInUseException(command.Email);
        }

        existingUser = await _userRepository.GetByPhoneAsync(command.Phone);
        if (existingUser != null)
        {
            throw new PhoneAlreadyInUseException(command.Phone);
        }

        var settings = new UserSettings(command.UserId, (Sex) command.DiscoverSex, 18, 35, 30, 0.0, 0.0);
        var user = new User(command.UserId, command.Phone, command.Email, command.Name, dob, (Sex) command.Sex, null, settings, command.Job, command.Bio);
        await _userRepository.AddAsync(user);
    }
}