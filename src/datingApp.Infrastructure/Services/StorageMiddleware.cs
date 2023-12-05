using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Application.Services;
using datingApp.Core.Exceptions;

namespace datingApp.Infrastructure.Exceptions;

internal sealed class StorageMiddleware : IMiddleware
{
    private readonly ILogger<IMiddleware> _logger;
    public StorageMiddleware(ILogger<IMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.ToString().StartsWith("/storage"))
        {
            string id = "";
            string ext = "";
            try
            {
                var s = context.Request.Path.ToString().Replace("/storage/", "").Split(".");
                id = s[0];
                ext = s[1];
            }
            catch (System.Exception)
            {
                // pass
            }
            if (!string.IsNullOrEmpty(id))
            {
                // pobieranie danych z bazy i zapisywanie na dysku
                // storageService.GetFile(id);
                _logger.LogInformation($"Storage access: {id}, {ext}");
            }
        }
        await next(context);
    }
}