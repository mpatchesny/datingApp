using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Services;
using datingApp.Core.Exceptions;
using datingApp.Infrastructure.Services;

namespace datingApp.Infrastructure.Exceptions;

internal sealed class StorageMiddleware : IMiddleware
{
    private readonly ILogger<IMiddleware> _logger;
    private readonly IFileStorage _dbFileStorage;
    private readonly FileStorageOptions _diskFileStorageOptions;
    public StorageMiddleware(ILogger<IMiddleware> logger, IFileStorage dbFileStorage, FileStorageOptions diskFileStorageOptions)
    {
        _logger = logger;
        _dbFileStorage = dbFileStorage;
        _diskFileStorageOptions = diskFileStorageOptions;
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
        if (!_diskFileStorageOptions.Exists(id, extension))
        {
            var file = await _dbFileStorage.GetFileAsync(id);
            if (file != null)
            {
                _diskFileStorageOptions.SaveFile(file, id, extension);
            }
        }
    }
}