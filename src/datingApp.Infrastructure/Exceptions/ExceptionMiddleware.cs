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

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
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
        var (statusCode, error) = exception switch 
        {
            UnauthorizedException => (StatusCodes.Status403Forbidden, new Error(GetPrettyExeptionName(exception.GetType().Name), exception.Message)),
            PhotoNotExistsException => (StatusCodes.Status404NotFound, new Error(GetPrettyExeptionName(exception.GetType().Name), exception.Message)),
            UserNotExistsException => (StatusCodes.Status404NotFound, new Error(GetPrettyExeptionName(exception.GetType().Name), exception.Message)),
            PhotoAlreadyDeletedException => (StatusCodes.Status410Gone, new Error(GetPrettyExeptionName(exception.GetType().Name), exception.Message)),
            UserAlreadyDeletedException => (StatusCodes.Status410Gone, new Error(GetPrettyExeptionName(exception.GetType().Name), exception.Message)),
            CustomException => (StatusCodes.Status400BadRequest, new Error(GetPrettyExeptionName(exception.GetType().Name), exception.Message)),
            _ => (StatusCodes.Status500InternalServerError, new Error("error", "Something went wrong.")),
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(error);
    }

    private record Error(string Code, string Reason);

    private string GetPrettyExeptionName(string exceptionName)
    {
        var re = new Regex("([A-Z])");
        var ex = exceptionName.Replace("Exception", "");
        ex = re.Replace(ex, "_$1").ToLowerInvariant();
        return ex.StartsWith("_") ? ex.Substring(1) : ex;
    }
}