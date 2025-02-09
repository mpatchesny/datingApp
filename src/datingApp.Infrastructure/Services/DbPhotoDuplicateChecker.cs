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
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            photo.Position = 0;
            var hashBytes = await md5.ComputeHashAsync(photo);
            var checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            var query = _dbContext
                .Users.AnyAsync(user => user.Id.Equals(userId) &&
                    user.Photos.Any(photo => photo.Checksum.Equals(checksum)));
            return await query;
        }
    }
}