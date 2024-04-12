using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class DeleteUserHandler : ICommandHandler<DeleteUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileStorage;
    public DeleteUserHandler(IUserRepository userRepository, IFileRepository fileStorage)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorage;
    }

    public async Task HandleAsync(DeleteUser command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        Task[] tasks = new Task[user.Photos.Count()+1];
        for (int i=0; i<user.Photos.Count(); i++)
        {
            tasks[i] = _fileStorage.DeleteAsync(user.Photos.ElementAt(i).Id.ToString());
        }
        tasks[tasks.Length - 1] = _userRepository.DeleteAsync(user);
        await Task.WhenAll(tasks);
    }
}