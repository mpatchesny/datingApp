using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface ISwipeRepository : IRepository
{
    Task AddAsync(Swipe swipe);
    Task<Swipe> GetBySwipedBy(Guid swipedById, Guid swipedWhoId);
    Task<bool> SwipeExists(Guid swipedById, Guid swipedWhoId);
}