using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Specifications;
using datingApp.Core.ValueObjects;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Npgsql.Replication;

namespace datingApp.Infrastructure.DAL.Repositories.Specifications;

public class MatchWithMessagesSpecification : ISpecification<Match>
{
    readonly List<Expression<Func<Message, bool>>> _predicates = new();
    private int messageLimit = int.MaxValue;

    public MatchWithMessagesSpecification()
    {
    }

    public MatchWithMessagesSpecification GetMessagesBeforeDate(DateTime createdBefore)
    {
        _predicates.Add(message => message.CreatedAt < createdBefore);
        return this;
    }

    public MatchWithMessagesSpecification GetMessageById(MessageId messageId)
    {
        _predicates.Add(message => message.Id == messageId);
        return this;
    }

    public MatchWithMessagesSpecification GetMessagesByDisplayed(bool isDisplayed)
    {
        _predicates.Add(message => message.IsDisplayed == isDisplayed);
        return this;
    }

    public MatchWithMessagesSpecification SetMessageFetchLimit(int messageLimit)
    {
        this.messageLimit = messageLimit;
        return this;
    }

    public IQueryable<Match> Apply(IQueryable<Match> query)
    {
        Expression<Func<Match, IEnumerable<Message>>> navigationPropertyPath = 
            match => match.Messages;

        if (_predicates.Count > 0)
        {
            navigationPropertyPath = match => match.Messages
                //.Where(_predicates[0])
                .OrderByDescending(message => message.CreatedAt)
                .Take(messageLimit);
        }

        return query.Include(navigationPropertyPath);
    }
}