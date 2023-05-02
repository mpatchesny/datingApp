using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Abstractions;

public interface IAuthenticatedQuery<TResult> : IQuery<TResult>
{
    public Guid AuthenticatedUserId { get; set; }
}