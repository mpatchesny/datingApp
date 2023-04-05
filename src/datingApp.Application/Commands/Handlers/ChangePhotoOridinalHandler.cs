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
        var thisPhoto = photos.FirstOrDefault(x => x.Id == command.PhotoId);
        if (thisPhoto == null)
        {
            throw new PhotoNotExistsException(command.PhotoId);
        }

        if (thisPhoto.Oridinal == command.NewOridinal)
        {
            return;
        }

        var photoList = photos.ToList<Photo>();
        if (command.NewOridinal > photoList.Count()-1)
        {
            photoList.RemoveAt(thisPhoto.Oridinal);
            photoList.Add(thisPhoto);
        }
        else if (command.NewOridinal < 0)
        {
            photoList.RemoveAt(thisPhoto.Oridinal);
            photoList.Insert(0, thisPhoto);
        }
        else
        {
            var newOridinal = command.NewOridinal;
            if (newOridinal > thisPhoto.Oridinal) newOridinal++;
            photoList.Insert(newOridinal, thisPhoto);
            int shift = (thisPhoto.Oridinal > newOridinal) ? 1 : 0;
            photoList.RemoveAt(thisPhoto.Oridinal + shift);
        }

        for (int i = 0; i < photoList.Count; i++)
        {
            photoList[i].ChangeOridinal(i);
            await _photoRepository.UpdateAsync(photoList[i]);
        }
    }
}