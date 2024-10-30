using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using FluentStorage;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Mvc.Routing;

namespace datingApp.Application.Commands.Handlers;

public sealed class AddPhotoHandler : ICommandHandler<AddPhoto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPhotoService _photoService;
    private readonly IBlobStorage _fileStorage;
    public AddPhotoHandler(IUserRepository userRepository,
                           IPhotoService photoService,
                           IBlobStorage fileStorage)
    {
        _userRepository = userRepository;
        _photoService = photoService;
        _fileStorage = fileStorage;
    }

    public async Task HandleAsync(AddPhoto command)
    {
        var (bytes, extension) = _photoService.ProcessBase64Photo(command.Base64Bytes);

        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user == null)
        {
            throw new UserNotExistsException(command.UserId);
        }

        var photoUrl = $"~/storage/{command.PhotoId}.{extension}";
        var photo = new Photo(command.PhotoId, command.UserId, photoUrl, 0);
        user.AddPhoto(photo);

        var path = $"{photo.Id}.{extension}";
        var tasks = new List<Task>(){
            _fileStorage.WriteAsync(path, new MemoryStream(bytes)),
            _userRepository.UpdateAsync(user)
        };
        await Task.WhenAll(tasks);
    }
}