using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication;

namespace datingApp.Infrastructure.DAL.Specifications;

public class MatchWithMessagesSpecification : BaseSpecification<Match>
{
    public MatchWithMessagesSpecification()
    {
        AddInclude(match => match.Messages);
    }

    public MatchWithMessagesSpecification GetMessagesBeforeDate(DateTime createdBefore)
    {
        AddCriteria(match => match.Messages.Any(m => m.CreatedAt <= createdBefore));
        return this;
    }

    public MatchWithMessagesSpecification GetOnlyNotDisplayedMessages()
    {
        AddCriteria(match => match.Messages.Any(m => !m.IsDisplayed));
        return this;
    }

    public MatchWithMessagesSpecification SetMessageLimit(int messageLimit)
    {
        Expression<Func<Match, object>> include = match => 
            match.Messages.OrderBy(message => message.CreatedAt).Take(messageLimit);
        AddInclude(include);
        return this;
    }

    public override IQueryable<Match> Apply(IQueryable<Match> query)
    {
        foreach (var include in Include)
        {
            query = query.Include(include);
        }

        foreach (var criteria in Criteria)
        {
            query = query.Where(criteria);
        }

        return query;
    }
}