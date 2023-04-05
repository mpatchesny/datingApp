using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.Exceptions;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Application.Commands.Handlers;

public sealed class ChangePhotoOridinalHandler : ICommandHandler<ChangePhotoOridinal>
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoOrderer _photoOrderer;
    public ChangePhotoOridinalHandler(IPhotoRepository photoRepository, IPhotoOrderer photoOrderer)
    {
        _photoRepository = photoRepository;
        _photoOrderer = photoOrderer;
    }

    public async Task HandleAsync(ChangePhotoOridinal command)
    {
        var photos = await _photoRepository.GetByUserIdAsync(command.UserId);
        var thisPhoto = photos.FirstOrDefault(x => x.Id == command.PhotoId);
        if (thisPhoto == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }

        if (thisPhoto.Oridinal == command.NewOridinal)
        {
            return;
        }

        var photoList = _photoOrderer.OrderPhotos(photos.ToList<Photo>(), thisPhoto.Id, command.NewOridinal);
        for (int i = 0; i < photoList.Count(); i++)
        {
            photoList[i].ChangeOridinal(i);
            await _photoRepository.UpdateAsync(photoList[i]);
        }
    }
}