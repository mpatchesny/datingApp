using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Specifications;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories.QueryObjects;

internal sealed class GetMatchesWithMessagesByMessageId : IQueryObject<Match>
{
    private readonly Expression<Func<Match, IOrderedEnumerable<Message>>> _navigationPropertyPath;
    private readonly MessageId _messageId;

    public GetMatchesWithMessagesByMessageId(MessageId messageId, bool? isDisplayed = null)
    {
        _messageId = messageId;
        _navigationPropertyPath = match => match.Messages
                    .Where(message => !isDisplayed.HasValue || message.IsDisplayed == isDisplayed.Value)
                    .OrderByDescending(message => message.CreatedAt);
    }

    public IQueryable<Match> Apply(IQueryable<Match> query)
    {
        return query
            .Where(match => match.Messages.Any(message => message.Id.Equals(_messageId)))
            .Include(_navigationPropertyPath);
    }
}