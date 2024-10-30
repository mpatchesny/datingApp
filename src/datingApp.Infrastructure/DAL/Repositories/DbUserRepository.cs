using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class DbUserRepository : IUserRepository
{
    private readonly DatingAppDbContext _dbContext;
    public DbUserRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
                        .FirstOrDefaultAsync(x=> x.Email == email.ToLowerInvariant().Trim());
    }

    public async Task<User> GetByIdAsync(UserId userId)
    {
        return await _dbContext.Users
                        .Include(x => x.Photos)
                        .Include(x => x.Settings)
                        .FirstOrDefaultAsync(x=> x.Id.Equals(userId));
    }

    public async Task<User> GetByPhoneAsync(string phone)
    {
        return await _dbContext.Users
                        .FirstOrDefaultAsync(x=> x.Phone == phone);
    }
    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdateAsync(User user)
    {
        var originalUser = _dbContext.Users.FirstOrDefault(x => x.Id == user.Id);
        _dbContext.Entry(originalUser).CurrentValues.SetValues(user);
        await _dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(User user)
    {
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetByPhotoIdAsync(PhotoId photoId)
    {
        return await _dbContext.Users
                        .Include(x => x.Photos)
                        .Include(x => x.Settings)
                        .Where(x => x.Photos.Any(p => p.Id == photoId))
                        .FirstOrDefaultAsync();
    }
}