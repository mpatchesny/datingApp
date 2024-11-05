using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Specifications;
using datingApp.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories.Specifications;

internal sealed class GetMatchesWithNotDisplayedMessagesByMessageIdQuery : IQueryObject<Match>
{
    IQueryable<Match> IQueryObject<Match>.Query => _query;
    private IQueryable<Match> _query;

    public GetMatchesWithNotDisplayedMessagesByMessageIdQuery(MessageId messageId)
    {
        _query = _query.Include(match => match.Messages
            .Where(message => message.Id.Equals(messageId.Value))
            .Where(message => message.IsDisplayed == false)
            .OrderByDescending(message => message.CreatedAt));
    }
}