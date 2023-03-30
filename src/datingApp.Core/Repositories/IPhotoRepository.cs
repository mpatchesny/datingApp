using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface IPhotoRepository : IRepository
{
    Task<IEnumerable<Photo>> GetByUserIdAsync(int userId);
    Task AddAsync(Photo photo);
    Task DeleteAsync(int photoId);
}