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
    private readonly IPhotoValidator<Stream> _photoValidator;
    private readonly IPhotoConverter _photoConverter;
    private readonly IBlobStorage _fileStorage;
    public AddPhotoHandler(IUserRepository userRepository,
                            IPhotoValidator<Stream> photoService,
                            IBlobStorage fileStorage,
                            IPhotoConverter photoConverter)
    {
        _userRepository = userRepository;
        _photoValidator = photoService;
        _fileStorage = fileStorage;
        _photoConverter = photoConverter;
    }

    public async Task HandleAsync(AddPhoto command)
    {
        _photoValidator.ValidateSize(command.PhotoStream);
        _photoValidator.ValidateExtension(command.PhotoStream, out var extension);

        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        var convertedPhotoStream = await _photoConverter.ConvertAsync(command.PhotoStream, targetFormat: "jpg");
        // FIXME: photoStorageUrlProvider.GetPhotoUrl(photoId, extension)

        var photoUrl = $"~/storage/{command.PhotoId}.{extension}";
        var photo = new Photo(command.PhotoId, photoUrl, 0);
        user.AddPhoto(photo);

        var path = $"{photo.Id}.{extension}";
        var tasks = new List<Task>(){
            _fileStorage.WriteAsync(path, convertedPhotoStream),
            _userRepository.UpdateAsync(user)
        };
        await Task.WhenAll(tasks);

        throw new NotImplementedException();
    }
}