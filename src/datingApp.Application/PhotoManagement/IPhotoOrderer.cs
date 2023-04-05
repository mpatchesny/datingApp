using datingApp.Core.Entities;

namespace datingApp.Application.PhotoManagement;

public interface IPhotoOrderer
{
    public List<Photo> OrderPhotos(List<Photo> photos, int photoToChangeId, int newOridinal);
}