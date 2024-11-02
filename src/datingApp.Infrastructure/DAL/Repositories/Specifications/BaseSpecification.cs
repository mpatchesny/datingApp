using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using datingApp.Core.Specifications;

namespace datingApp.Infrastructure.DAL.Repositories.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public List<Expression<Func<T, bool>>> Criteria { get; } = new List<Expression<Func<T, bool>>>();
    public List<Expression<Func<T, object>>> Include { get; } = new List<Expression<Func<T, object>>>();
    public int? Take { get; private set; }

    protected void AddCriteria(Expression<Func<T, bool>> criteria) => Criteria.Add(criteria);
    protected void AddInclude(Expression<Func<T, object>> include) => Include.Add(include);
    protected void SetTake(int take) => Take = take;

    public abstract IQueryable<T> Apply(IQueryable<T> query);
}