using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class AddPhotoHandler : ICommandHandler<AddPhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPhotoService _photoService;
    public AddPhotoHandler(IPhotoRepository photoRepository, IUserRepository userRepository, IPhotoService photoService)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
        _photoService = photoService;
    }

    public async Task HandleAsync(AddPhoto command)
    {
        // FIXME: move min, maxPhotoSize to app settings
        // 2MB
        const int maxPhotoSizeBytes = 2*1024*1024;
        const int maxPhotoSizeMB = (int) maxPhotoSizeBytes / (1024*1024);
        const int minPhotoSizeBytes = 100*1024;
        const int minPhotoSizeKB = (int) minPhotoSizeBytes / 1024;
        if (command.Bytes.Count() > maxPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }
        if (command.Bytes.Count() < minPhotoSizeBytes)
        {
            throw new InvalidPhotoSizeException(minPhotoSizeKB, maxPhotoSizeMB);
        }

        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        if (user.Photos.Count() >= 6)
        {
            throw new UserPhotoLimitException();
        }

        int oridinal = user.Photos.Count();
        var photoPath = _photoService.SavePhoto(command.Bytes);
        var photo = new Photo(0, command.UserId, photoPath, oridinal);
        await _photoRepository.AddAsync(photo);
    }
}