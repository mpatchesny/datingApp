using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Application.PhotoManagement;

public class PhotoOrderer : IPhotoOrderer
{
    public List<Photo> OrderPhotos(List<Photo> photos, int photoToChangeId, int newOridinal)
    {
        var thisPhoto = photos.FirstOrDefault(x => x.Id == photoToChangeId);

        if (newOridinal > photos.Count()-1)
        {
            photos.RemoveAt(thisPhoto.Oridinal);
            photos.Add(thisPhoto);
        }
        else if (newOridinal < 0)
        {
            photos.RemoveAt(thisPhoto.Oridinal);
            photos.Insert(0, thisPhoto);
        }
        else
        {
            if (newOridinal > thisPhoto.Oridinal) newOridinal++;
            photos.Insert(newOridinal, thisPhoto);
            int shift = (thisPhoto.Oridinal > newOridinal) ? 1 : 0;
            photos.RemoveAt(thisPhoto.Oridinal + shift);
        }

        return photos;
    }
}