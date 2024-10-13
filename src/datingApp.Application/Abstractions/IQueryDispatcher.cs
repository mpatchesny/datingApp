using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Abstractions;

public interface IQueryDispatcher
{
    public Task<TResult> DispatchAsync<TQuery, TResult>(TQuery command) where TQuery: class, IQuery<TResult>;
}