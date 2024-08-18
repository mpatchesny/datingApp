using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Exceptions;
using datingApp.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Handlers;

internal sealed class GetMessageHandler : IQueryHandler<GetMessage, MessageDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetMessageHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MessageDto> HandleAsync(GetMessage query)
    {
        var message = await _dbContext.Messages
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.Id == query.MessageId);
        
        if (message == null) 
        {
            throw new MessageNotExistsException(query.MessageId);
        }
        
        return message.AsDto();
    }
}