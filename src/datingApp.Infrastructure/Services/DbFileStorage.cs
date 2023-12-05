using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.Services;

internal sealed class DbFileStorage : IFileStorage
{
    private readonly DatingAppDbContext _dbContext;
    public DbFileStorage(DatingAppDbContext dbContext)
    {
         _dbContext = dbContext;
    }

    public async Task DeleteFileAsync(string identification)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == identification);
        if (file != null)
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<byte[]> GetFileAsync(string identification)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == identification);
        if (file != null)
        {
            return file.Binary;
        }
        return null;
    }

    public async Task SaveFileAsync(byte[] photo, string identification, string extension)
    {
        var file = new FileDto {
            Id=identification,
            Extension = extension,
            Binary = photo
        };
        if (await _dbContext.Files.AnyAsync(x => x.Id == identification))
        {
            _dbContext.Files.Update(file);
        }
        else
        {
            await _dbContext.Files.AddAsync(file);
        }
        await _dbContext.SaveChangesAsync();
    }
}