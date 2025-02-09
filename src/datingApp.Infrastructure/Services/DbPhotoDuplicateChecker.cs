using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.Services;

internal sealed class DbPhotoDuplicateChecker : IPhotoDuplicateChecker
{
    private readonly DatingAppDbContext _dbContext;
    public DbPhotoDuplicateChecker(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsDuplicate(Guid userId, Stream photo)
    {
        var checksum = await ComputeHashAsync(photo);
        var query = _dbContext
            .Users.AnyAsync(user => user.Id.Equals(userId) &&
                user.Photos.Any(photo => photo.Checksum.Equals(checksum)));
        return await query;
    }

    private static async Task<string> ComputeHashAsync(Stream stream)
    {
        stream.Position = 0;
        var hashBytes = await System.Security.Cryptography.MD5.Create()
            .ComputeHashAsync(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}