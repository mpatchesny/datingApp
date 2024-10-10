using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Application.Exceptions;
using datingApp.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace datingApp.Infrastructure.Exceptions;

internal sealed class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly Dictionary<Type, int> _exceptionToHttpStatusCode;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _exceptionToHttpStatusCode = new Dictionary<Type, int>()
        {
            {typeof(UnauthorizedException), StatusCodes.Status403Forbidden},
            {typeof(NotExistsException), StatusCodes.Status404NotFound},
            {typeof(AlreadyDeletedException), StatusCodes.Status410Gone},
            {typeof(InvalidRefreshTokenException), StatusCodes.Status401Unauthorized},
            {typeof(CustomException), StatusCodes.Status400BadRequest},
        };
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(exception, context);
            string error = exception.ToString();
            if (exception is CustomException) 
            {
                _logger.LogInformation(error);
            }
            else
            {
                _logger.LogError(error);
            };
        }
    }

    private async Task HandleExceptionAsync(Exception exception, HttpContext context)
    {
        var statusCode = _exceptionToHttpStatusCode.GetValueOrDefault(exception.GetType(), 
            _exceptionToHttpStatusCode.GetValueOrDefault(exception.GetType().BaseType, StatusCodes.Status500InternalServerError));

        var error = new Error("error", "Something went wrong.");
        if (statusCode != StatusCodes.Status500InternalServerError)
        {
            error = new Error(GetPrettyExeptionName(exception.GetType().Name), exception.Message);
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(error);
    }

    private record Error(string Code, string Reason);

    private static string GetPrettyExeptionName(string exceptionName)
    {
        var re = new Regex("([A-Z])");
        var ex = exceptionName.Replace("Exception", "");
        ex = re.Replace(ex, "_$1").ToLowerInvariant();
        return ex.StartsWith("_") ? ex.Substring(1) : ex;
    }
}