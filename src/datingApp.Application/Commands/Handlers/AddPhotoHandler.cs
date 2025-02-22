using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Services;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using FluentStorage.Blobs;

namespace datingApp.Application.Commands.Handlers;

public sealed class AddPhotoHandler : ICommandHandler<AddPhoto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPhotoValidator _photoValidator;
    private readonly IPhotoConverter _jpegPhotoConverter;
    private readonly IPhotoDuplicateChecker _duplicateChecker;
    private readonly IBlobStorage _fileStorage;
    private readonly IPhotoUrlProvider _photoStorageUrlProvider;

    public AddPhotoHandler(IUserRepository userRepository,
                            IPhotoValidator photoValidator,
                            IBlobStorage fileStorage,
                            IPhotoConverter jpegPhotoConverter,
                            IPhotoUrlProvider photoStorageUrlProvider,
                            IPhotoDuplicateChecker duplicateChecker)
    {
        _userRepository = userRepository;
        _photoValidator = photoValidator;
        _fileStorage = fileStorage;
        _jpegPhotoConverter = jpegPhotoConverter;
        _photoStorageUrlProvider = photoStorageUrlProvider;
        _duplicateChecker = duplicateChecker;
    }

    public async Task HandleAsync(AddPhoto command)
    {
        _photoValidator.Validate(command.PhotoStream, out var extension);

        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        if (await _duplicateChecker.IsDuplicate(command.UserId, command.PhotoStream))
        {
            throw new PhotoAlreadyExistsException();
        }

        var convertedPhotoStream = await _jpegPhotoConverter.ConvertAsync(command.PhotoStream);
        var photoUrl = _photoStorageUrlProvider.GetPhotoUrl(command.PhotoId.ToString(), extension);

        var photo = new Photo(command.PhotoId, photoUrl, await ComputeHashAsync(command.PhotoStream), 0);
        user.AddPhoto(photo);

        var path = $"{photo.Id}.{extension}";
        var tasks = new List<Task>(){
            _fileStorage.WriteAsync(path, convertedPhotoStream),
            _userRepository.UpdateAsync(user)
        };
        await Task.WhenAll(tasks);
    }

    private static async Task<string> ComputeHashAsync(Stream stream)
    {
        stream.Position = 0;
        var hashBytes = await System.Security.Cryptography.MD5.Create()
            .ComputeHashAsync(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}