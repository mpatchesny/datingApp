using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Security;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class ChangePhotoOridinalHandler : ICommandHandler<ChangePhotoOridinal>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoOrderer _photoOrderer;
    private readonly IDatingAppAuthorizationService _authorizationService;

    public ChangePhotoOridinalHandler(IPhotoRepository photoRepository, IPhotoOrderer photoOrderer, IDatingAppAuthorizationService authorizationService)
    {
        _photoRepository = photoRepository;
        _photoOrderer = photoOrderer;
        _authorizationService = authorizationService;
    }

    public async Task HandleAsync(ChangePhotoOridinal command)
    {
        var thisPhoto = await _photoRepository.GetByIdAsync(command.PhotoId);
        if (thisPhoto == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(command.AuthenticatedUserId, thisPhoto, "OwnerPolicy");
        if (!authorizationResult.Succeeded)
        {
            throw new UnauthorizedException();
        }

        if (thisPhoto.Oridinal == command.NewOridinal)
        {
            return;
        }

        var photos = await _photoRepository.GetByUserIdAsync(thisPhoto.UserId);

        var photoList = _photoOrderer.OrderPhotos(photos.ToList<Photo>(), thisPhoto.Id, command.NewOridinal);
        for (int i = 0; i < photoList.Count(); i++)
        {
            photoList[i].ChangeOridinal(i);
        }

        await _photoRepository.UpdateRangeAsync(photoList.ToArray());
    }
}