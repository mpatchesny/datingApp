using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresUserRepository : IUserRepository
{
    private readonly DatingAppDbContext _dbContext;
    public PostgresUserRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IQueryable<User> GetAll()
    {
        return _dbContext.Users.AsQueryable();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
                        .FirstOrDefaultAsync(x=> x.Email == email.ToLowerInvariant().Trim());
    }

    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _dbContext.Users
                        .Include(x => x.Photos)
                        .Include(x => x.Settings)
                        .FirstOrDefaultAsync(x=> x.Id == userId);
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
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(User user)
    {
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }
}