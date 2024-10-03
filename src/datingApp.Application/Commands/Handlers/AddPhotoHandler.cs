using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class AddPhotoHandler : ICommandHandler<AddPhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;
    public AddPhotoHandler(IPhotoRepository photoRepository,
                            IUserRepository userRepository)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(AddPhoto command)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        if (user.Photos.Count() >= 6)
        {
            throw new UserPhotoLimitException();
        }

        var photoUrl = $"~/storage/{command.PhotoId}";
        int oridinal = user.Photos.Count();
        var photoFile = new PhotoFile(command.PhotoId, command.Base64Bytes);
        var photo = new Photo(command.PhotoId, command.UserId, "depreciated", photoUrl, oridinal, photoFile);

        await _photoRepository.AddAsync(photo);
    }
}