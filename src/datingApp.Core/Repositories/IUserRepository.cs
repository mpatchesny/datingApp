using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Repositories;

public interface IUserRepository : IRepository
{
    Task<User> GetByIdAsync(UserId userId);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByPhoneAsync(string phone);
    Task<User> GetByPhotoIdAsync(PhotoId photoId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}
