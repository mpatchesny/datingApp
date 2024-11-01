using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace datingApp.Core.Specifications;

public interface ISpecification<T>
{
    List<Expression<Func<T, bool>>> Criteria { get; }
    List<Expression<Func<T, object>>> Include { get; }
    int? Take { get; }

    public IQueryable<T> Apply(IQueryable<T> query);
}