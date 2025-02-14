using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Repositories;

public interface ISwipeRepository : IRepository
{
    Task AddAsync(Swipe swipe);
    Task<List<Swipe>> GetBySwipedBySwipedWho(UserId swipedById, UserId swipedWhoId);
    Task DeleteUserSwipes(UserId swipedById);
}