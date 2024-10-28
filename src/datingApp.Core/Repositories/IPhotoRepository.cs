using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Repositories;

public interface IPhotoRepository : IRepository
{
    Task<Photo> GetByIdAsync(PhotoId photoId);
    Task<IEnumerable<Photo>> GetByUserIdAsync(UserId userId);
    Task AddAsync(Photo photo);
    Task DeleteAsync(Photo photo);
    Task UpdateAsync(Photo thisPhoto);
    Task UpdateRangeAsync(Photo[] photos);
}