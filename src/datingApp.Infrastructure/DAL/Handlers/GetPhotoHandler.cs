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

internal sealed class GetPhotoHandler : IQueryHandler<GetPhoto, PhotoDto>
{
    private readonly DatingAppDbContext _dbContext;
    public GetPhotoHandler(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<PhotoDto> HandleAsync(GetPhoto query)
    {
        var photo = await _dbContext.Photos
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == query.PhotoId);
        
        if (photo == null)
        {
            throw new PhotoNotExistsException(query.PhotoId);
        }

        return photo.AsDto();
    }
}