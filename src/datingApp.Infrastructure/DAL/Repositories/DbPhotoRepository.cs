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
        // var compressed = _dbContext
        //                         .PhotoFiles
        //                         .Where(p => p.PhotoId == photoId)
        //                         .Select(p => p.Content)
        //                         .SingleOrDefault();
        // _fileCompressor.Decompress(compressed, out byte[] decompressed);
        return await _dbContext.Photos.Include(p => p.File).FirstOrDefaultAsync(x => x.Id == photoId);;
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
        await _dbContext.Photos.AddAsync(photo);
        // compressing?
        // _fileCompressor.Compress(photo.File.Content, out byte[] compressed);
        // var photoFileCopy = new PhotoFile(photo.Id, compressed);
        // var photoCopy = new Photo(photo.Id, photo.UserId, photo.Url, photo.Oridinal, photoFileCopy);
        // _dbContext.PhotoFiles
        //     .Where(p => p.PhotoId == photo.Id)
        //     .ExecuteUpdate(b => b.SetProperty(p => p.Content, compressed));
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