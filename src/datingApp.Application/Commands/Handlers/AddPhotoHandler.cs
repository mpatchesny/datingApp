using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class AddPhotoHandler : ICommandHandler<AddPhoto>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPhotoService _photoService;
    private readonly IFileStorageService _fileStorage;
    public AddPhotoHandler(IPhotoRepository photoRepository,
                            IUserRepository userRepository,
                            IPhotoService photoService,
                            IFileStorageService fileStorage)
    {
        _photoRepository = photoRepository;
        _userRepository = userRepository;
        _photoService = photoService;
        _fileStorage = fileStorage;
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

        var (bytes, extension) = _photoService.ProcessBase64Photo(command.Base64Bytes);

        var photoUrl = $"~/storage/{command.PhotoId}.{extension}";
        int oridinal = user.Photos.Count();
        var photo = new Photo(command.PhotoId, command.UserId, photoUrl, oridinal, null);

        await _photoRepository.AddAsync(photo);
        _fileStorage.SaveFile(bytes, command.PhotoId.ToString(), extension);
    }
}