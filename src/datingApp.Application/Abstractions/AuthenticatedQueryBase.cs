using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Abstractions;

public class AuthenticatedQueryBase<TResult> : IAuthenticatedQuery<TResult>
{
    public Guid AuthenticatedUserId { get; set; }
}