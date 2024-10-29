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
    Task<Swipe> GetBySwipedBy(UserId swipedById, UserId swipedWhoId);
    Task<bool> SwipeExists(UserId swipedById, UserId swipedWhoId);
}