using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL.Repositories;

internal sealed class DbFileRepository : IFileRepository
{
    private readonly DatingAppDbContext _dbContext;
    private readonly IFileCompressor _fileCompressor;
    public DbFileRepository(DatingAppDbContext dbContext, IFileCompressor fileCompressor)
    {
        _dbContext = dbContext;
        _fileCompressor = fileCompressor;
    }

    public async Task<bool> ExistsAsync(Guid fileId)
    {
        return await _dbContext.Files.AnyAsync(x => x.Id == fileId);
    }

    public async Task<byte[]> GetByIdAsync(Guid fileId)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
        if (file == null) return null;
        _fileCompressor.Decompress(file.Binary, out byte[] decompressedBinary);
        return decompressedBinary;
    }

    public async Task AddAsync(byte[] photo, Guid fileId, string extension)
    {
        _fileCompressor.Compress(photo, out byte[] compressedBinary);

        var file = new FileDto {
            Id = fileId,
            Extension = extension,
            Binary = compressedBinary
        };

        var existingFile = await _dbContext.Files.FirstOrDefaultAsync(x => x.Id == fileId);
        if (existingFile == null)
        {
            await _dbContext.Files.AddAsync(file);
        }
        else
        {
            existingFile.Extension = file.Extension;
            existingFile.Binary = file.Binary;
            _dbContext.Files.Update(existingFile);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid fileId)
    {
        await _dbContext.Files.Where(f => f.Id == fileId).ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
    }
}