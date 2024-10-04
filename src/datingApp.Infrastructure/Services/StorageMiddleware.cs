using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Repositories;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Core.Exceptions;
using datingApp.Core.Repositories;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Services;

namespace datingApp.Infrastructure.Exceptions;

internal sealed class StorageMiddleware : IMiddleware
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IFileStorageService _diskFileService;
    public StorageMiddleware(IPhotoRepository photoRepository, IFileStorageService diskFileService)
    {
        _photoRepository = photoRepository;
        _diskFileService = diskFileService;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.ToString().Contains("/storage"))
        {
            var s = context.Request.Path.ToString()["/storage/".Length..].Split(".");
            if (s.Length == 2)
            {
                await GetFileFromDatabaseAndSaveLocallyIfNotExists(s[0], s[1]);
            }
        }
        await next(context);
    }

    private async Task GetFileFromDatabaseAndSaveLocallyIfNotExists(string id, string extension)
    {
        if (!_diskFileService.Exists(id, extension))
        {
            var photo = await _photoRepository.GetByIdWithFileAsync(Guid.Parse(id));
            if (photo != null)
            {
                _diskFileService.SaveFile(photo.File.Content, id, extension);
            }
        }
    }
}