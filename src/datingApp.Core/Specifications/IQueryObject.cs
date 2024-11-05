using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Specifications;

public interface IQueryObject<T> where T: class
{
    IQueryable<T> Query { get; }
}