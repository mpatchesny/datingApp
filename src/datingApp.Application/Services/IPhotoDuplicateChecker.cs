using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IPhotoDuplicateChecker
{
    public Task<bool> IsDuplicate(Guid userId, Stream photo);
}