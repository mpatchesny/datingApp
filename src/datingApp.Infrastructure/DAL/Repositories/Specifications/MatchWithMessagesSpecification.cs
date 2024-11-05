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
    private Expression<Func<Match, IEnumerable<Message>>> navigationPropertyPath;
    private DateTime createdBefore = DateTime.MaxValue;
    private bool? isDisplayed;
    private Guid? messageId;
    private int messageLimit = int.MaxValue;

    public MatchWithMessagesSpecification()
    {
        navigationPropertyPath = match => match.Messages;
    }

    public MatchWithMessagesSpecification GetMessagesBeforeDate(DateTime createdBefore)
    {
        this.createdBefore = createdBefore;
        return this;
    }

    public MatchWithMessagesSpecification GetMessageById(MessageId messageId)
    {
        this.messageId = messageId;
        return this;
    }

    public MatchWithMessagesSpecification GetMessagesByDisplayed(bool isDisplayed)
    {
        this.isDisplayed = isDisplayed;
        return this;
    }

    public MatchWithMessagesSpecification SetMessageFetchLimit(int messageLimit)
    {
        this.messageLimit = messageLimit;
        return this;
    }

    public IQueryable<Match> Apply(IQueryable<Match> query)
    {
        navigationPropertyPath = match => match.Messages
            .Where(message => message.CreatedAt <= createdBefore)
            .Where(message => !isDisplayed.HasValue || message.IsDisplayed == isDisplayed.Value)
            .Where(message => !messageId.HasValue || message.Id.Equals(messageId.Value))
            .OrderByDescending(message => message.CreatedAt)
            .Take(messageLimit);
        return query.Include(navigationPropertyPath);
    }
}