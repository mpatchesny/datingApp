using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class DbFileRepository : IFileRepository
{
    private readonly DatingAppDbContext _dbContext;
    public DbFileRepository(DatingAppDbContext dbContext)
    {
         _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(Guid fileId)
    {
        return await _dbContext.Files.AnyAsync(x => x.Id == fileId);
    }

    public async Task<byte[]> GetByIdAsync(Guid fileId)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
        return file?.Binary;
    }

    public async Task AddAsync(byte[] photo, Guid fileId, string extension)
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

    public async Task DeleteAsync(Guid fileId)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
        if (file != null)
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync();
        }
    }

}