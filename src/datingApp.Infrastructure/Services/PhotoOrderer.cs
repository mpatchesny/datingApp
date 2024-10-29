using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using datingApp.Core.Entities;

namespace datingApp.Infrastructure.Services;

public class PhotoOrderer : IPhotoOrderer
{
    public List<Photo> OrderPhotos(List<Photo> photos, Guid photoToChangeId, int newOridinal)
    {
        // deep copy
        var orderedPhotos = photos.Select(photo => 
                new Photo(photo.Id, photo.UserId, photo.Url, photo.Oridinal)
            ).ToList();
        var thisPhoto = orderedPhotos.FirstOrDefault(x => x.Id.Equals(photoToChangeId));

        if (newOridinal >= orderedPhotos.Count - 1)
        {
            // insert at the end
            orderedPhotos.RemoveAt(thisPhoto.Oridinal);
            orderedPhotos.Add(thisPhoto);
        }
        else if (newOridinal <= 0)
        {
            // insert at the beginning
            orderedPhotos.RemoveAt(thisPhoto.Oridinal);
            orderedPhotos.Insert(0, thisPhoto);
        }
        else
        {
            // inesrt in the middle
            if (newOridinal > thisPhoto.Oridinal) newOridinal++;
            orderedPhotos.Insert(newOridinal, thisPhoto);
            int shift = (thisPhoto.Oridinal > newOridinal) ? 1 : 0;
            orderedPhotos.RemoveAt(thisPhoto.Oridinal + shift);
        }

        return orderedPhotos;
    }
}