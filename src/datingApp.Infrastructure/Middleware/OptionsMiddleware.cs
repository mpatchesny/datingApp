using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Middleware;

internal sealed class OptionsMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
        }
    }
}