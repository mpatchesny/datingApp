using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Security;

public interface IResourceAuthorizationService
{
    public Task<bool> AuthorizeAsync(Guid userId, object? resource);
}