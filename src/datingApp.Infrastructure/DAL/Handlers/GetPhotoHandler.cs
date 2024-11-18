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
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Photos
                .Where(photo => photo.Id.Equals(query.PhotoId)))
            .FirstOrDefaultAsync();

        if (user == null || !user.Photos.Any())
        {
            throw new PhotoNotExistsException(query.PhotoId);
        }

        var photo = user.Photos.FirstOrDefault();
        return new PhotoDto
        {
            Id = photo.Id,
            UserId = user.Id,
            Url = photo.Url,
            Oridinal = photo.Oridinal
        };
    }
}