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
    public StorageMiddleware(ILogger<IMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.ToString().StartsWith("/storage"))
        {
            var s = context.Request.Path.ToString().Replace("/storage/", "").Split(".");
            if (s.Length == 1) 
            {
                string id = s[0];
                string ext = s[1];
                await GetFileFromDatabaseAndSaveLocallyIfNotExists(id, ext);
                _logger.LogInformation($"Storage access: {id}, {ext}");
            }
        }
        await next(context);
    }

    private async Task GetFileFromDatabaseAndSaveLocallyIfNotExists(string id, string extension)
    {
        if (!_diskFileStorageOptions.Exists(id))
        {
            var file = await _dbFileStorage.GetFileAsync(id);
            if (file != null)
            {
                _diskFileStorageOptions.SaveFile(file, id, extension);
            }
        }
    }
}