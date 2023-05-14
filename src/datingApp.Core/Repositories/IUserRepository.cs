using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface IUserRepository : IRepository
{
    Task<User> GetByIdAsync(Guid userId);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByPhoneAsync(string phone);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task UpdateSettingsAsync(User user);
    Task DeleteAsync(User user);
}
