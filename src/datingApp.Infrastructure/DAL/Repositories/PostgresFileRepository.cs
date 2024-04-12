using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class PostgresFileRepository : IFileRepository
{
    private readonly DatingAppDbContext _dbContext;
    public PostgresFileRepository(DatingAppDbContext dbContext)
    {
         _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(string fileId)
    {
        return await _dbContext.Files.AnyAsync(x => x.Id == fileId);
    }

    public async Task<byte[]> GetByIdAsync(string fileId)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
        if (file != null)
        {
            return file.Binary;
        }
        return null;
    }

    public async Task SaveFileAsync(byte[] photo, string fileId, string extension)
    {
        var file = new FileDto {
            Id = fileId,
            Extension = extension,
            Binary = photo
        };
        if (await _dbContext.Files.AnyAsync(x => x.Id == fileId))
        {
            var originalFile = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
            _dbContext.Entry(originalFile).CurrentValues.SetValues(file);
        }
        else
        {
            await _dbContext.Files.AddAsync(file);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string fileId)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
        if (file != null)
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync();
        }
    }

}