using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Core.Repositories;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMessagesHandler : IQueryHandler<GetMessages, IEnumerable<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    public GetMessagesHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IEnumerable<MessageDto>> HandleAsync(GetMessages query)
    {
        var messages = await _messageRepository.GetByMatchIdAsync(query.MatchId);
        return messages.Select(x => x.AsDto()).ToList();
    }
}