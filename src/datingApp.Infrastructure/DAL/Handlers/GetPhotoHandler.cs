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
    private readonly ReadOnlyDatingAppDbContext _dbContext;
    public GetPhotoHandler(ReadOnlyDatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PhotoDto> HandleAsync(GetPhoto query)
    {
        var user = await _dbContext.Users
            .Include(user => user.Photos
                .Where(photo => photo.Id.Equals(query.PhotoId)))
            .FirstOrDefaultAsync();

        if (user == null || user.Photos.Count == 0)
        {
            throw new PhotoNotExistsException(query.PhotoId);
        }

        return user.PhotosAsDto().FirstOrDefault();
    }
}