using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
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
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        if (user.Photos.Count() >= 6)
        {
            throw new UserPhotoLimitException();
        }

        byte[] bytes = _photoService.ConvertToArrayOfBytes(command.Base64Bytes);
        _photoService.ValidatePhoto(bytes);
        var extension = _photoService.GetImageFileFormat(bytes);
        var photoPath = _photoService.SavePhoto(bytes, extension);
        int oridinal = user.Photos.Count();
        var photo = new Photo(command.PhotoId, command.UserId, photoPath, oridinal);
        await _photoRepository.AddAsync(photo);
    }
}