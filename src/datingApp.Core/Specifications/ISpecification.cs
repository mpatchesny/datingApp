using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace datingApp.Core.Specifications;

public interface ISpecification<T>
{
    public IQueryable<T> Apply(IQueryable<T> query);
}