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
    
    public async Task<IQueryable<User>> GetAllAsync()
    {
        return _dbContext.Users.AsQueryable<User>();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x=> x.Email == email);
    }

    public async Task<User> GetByIdAsync(int userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x=> x.Id == userId);
    }

    public async Task<User> GetByPhoneAsync(string phone)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x=> x.Phone == phone);
    }
    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }
    public Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }
    public async Task DeleteAsync(int userId)
    {
        var user = await _dbContext.Users.FirstAsync(x => x.Id == userId);
        _dbContext.Users.Remove(user);
    }
}