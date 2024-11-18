using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Repositories;

public interface IDeletedEntityService
{
    public Task<bool> ExistsAsync(Guid id);
    public Task AddAsync(Guid id);
}