using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;
    public InMemoryUserRepository(List<User> users)
    {
        _users = users;
    }

    public async Task<User> GetByIdAsync(int userId)
    {
        await Task.CompletedTask;
        return _users.SingleOrDefault(x => x.Id == userId);
    }
    public async Task<User> GetByEmailAsync(string email)
    {
        await Task.CompletedTask;
        return _users.SingleOrDefault(x => x.Email == email);
    }
    public async Task<User> GetByPhoneAsync(string phone)
    {
        await Task.CompletedTask;
        return _users.SingleOrDefault(x => x.Phone == phone);
    }
    public async Task AddAsync(User user)
    {
        await Task.CompletedTask;
        _users.Add(user);
    }
    public Task UpdateAsync(User user) => Task.CompletedTask;
    public async Task DeleteAsync(User user)
    {
        await Task.CompletedTask;
        _users.Remove(user);
    }
}