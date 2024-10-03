using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using datingApp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class DbPhotoRepository : IPhotoRepository
{
    private readonly DatingAppDbContext _dbContext;
    public DbPhotoRepository(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Photo> GetByIdAsync(Guid photoId)
    {
        return await _dbContext.Photos.FirstOrDefaultAsync(x => x.Id == photoId);
    }

    public async Task<Photo> GetByIdWithFileAsync(Guid photoId)
    {
        // decompressing?
        // _fileCompressor.Compress(photo.File.Content, out byte[] compressedBinary);
        return await _dbContext.Photos.Include(f => f.File).FirstOrDefaultAsync(x => x.Id == photoId);
    }

    public async Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.Photos
                    .Where(x=> x.UserId == userId)
                    .OrderBy(p => p.Oridinal)
                    .ToListAsync();
    }

    public async Task AddAsync(Photo photo)
    {
        // compressing?
        // _fileCompressor.Compress(photo, out byte[] compressedBinary);
        // var photoFileCopy = new PhotoFile(photo.Id, compressedBinary);
        // var photoCopy = new Photo(photo.Id, photo.UserId, photo.Url, photo.Oridinal, photoFileCopy);
        await _dbContext.Photos.AddAsync(photo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Photo photo)
    {
        _dbContext.Photos.Update(photo);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(Photo[] photos)
    {
        _dbContext.Photos.UpdateRange(photos);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Photo photo)
    {
        _dbContext.Photos.Remove(photo);
        await _dbContext.SaveChangesAsync();
    }


}