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
    public ChangePhotoOridinalHandler(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task HandleAsync(ChangePhotoOridinal command)
    {
        var photos = await _photoRepository.GetByUserIdAsync(command.UserId);
        
        Photo thisPhoto = null;
        Photo otherPhoto = null;
        foreach (var photo in photos)
        {
            if (photo.Id == command.PhotoId)
            {
                thisPhoto = photo;
            }
            else if (photo.Oridinal == command.NewOridinal)
            {
                otherPhoto = photo;
            }
        }

        if (thisPhoto == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }
        else if (thisPhoto.Oridinal == command.NewOridinal)
        {
             return;
        }

        thisPhoto.ChangeOridinal(command.NewOridinal);
        await _photoRepository.UpdateAsync(thisPhoto);
        if (otherPhoto != null)
        {
            otherPhoto.ChangeOridinal(thisPhoto.Oridinal);
            await _photoRepository.UpdateAsync(otherPhoto);
        }
    }
}