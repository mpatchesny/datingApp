using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface IPhotoRepository : IRepository
{
    Task<Photo> GetByIdAsync(Guid photoId);
    Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Photo photo);
    Task DeleteAsync(Photo photo);
    Task UpdateAsync(Photo thisPhoto);
}