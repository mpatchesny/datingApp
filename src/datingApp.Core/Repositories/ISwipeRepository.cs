using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;

namespace datingApp.Core.Repositories;

public interface ISwipeRepository : IRepository
{
    Task AddAsync(Swipe swipe);
    Task<IEnumerable<Swipe>> GetByUserIdAsync(Guid userId1, Guid userId2);
    Task<Swipe> GetBySwipedBySwipedWhoAsync(Guid swipedById, Guid swipedWhoId);
}