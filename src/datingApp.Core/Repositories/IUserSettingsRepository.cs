using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface IUserSettingsRepository
{
    Task<UserSettings> GetByIdAsync(int userId);
    Task AddAsync(UserSettings settings);
    Task UpdateAsync(UserSettings settings);
}