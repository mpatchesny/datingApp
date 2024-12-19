using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Castle.Core.Logging;
using datingApp.Application.Exceptions;
using datingApp.Core.Exceptions;
using datingApp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Unit.Middleware;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async void given_no_exception_thrown_ExceptionMiddleware_returns_200_status_code()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();
        var exception = new InvalidDateOfBirthException("message");
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>()));

        await middleware.InvokeAsync(context, next.Object);

        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async void given_custom_exception_is_thrown_ExceptionMiddleware_returns_400_status_code()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();
        var exception = new InvalidDateOfBirthException("date of birth is invalid");
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(exception);

        await middleware.InvokeAsync(context, next.Object);

        Assert.Equal(400, context.Response.StatusCode);
    }

    [Fact]
    public async void given_not_custom_exception_is_thrown_ExceptionMiddleware_returns_500_status_code()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();
        var exception = new ArgumentNullException();
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(exception);

        await middleware.InvokeAsync(context, next.Object);

        Assert.Equal(500, context.Response.StatusCode);
    }

    [Fact]
    public async void given_UnauthorizedException_is_thrown_ExceptionMiddleware_returns_403_status_code()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();
        var exception = new UnauthorizedException();
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(exception);

        await middleware.InvokeAsync(context, next.Object);

        Assert.Equal(403, context.Response.StatusCode);
    }

    [Fact]
    public async void given_NotExistsException_is_thrown_ExceptionMiddleware_returns_404_status_code()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();
        var exception = new NotExistsException("message");
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(exception);

        await middleware.InvokeAsync(context, next.Object);

        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public async void given_AlreadyDeletedException_is_thrown_ExceptionMiddleware_returns_410_status_code()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();
        var exception = new AlreadyDeletedException("message");
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(exception);

        await middleware.InvokeAsync(context, next.Object);

        Assert.Equal(410, context.Response.StatusCode);
    }

    [Fact]
    public async void given_InvalidRefreshTokenException_is_thrown_ExceptionMiddleware_returns_401_status_code()
    {
        var logger = new Mock<ILogger<ExceptionMiddleware>>();
        var middleware = new ExceptionMiddleware(logger.Object);
        var context = new DefaultHttpContext();
        var next = new Mock<RequestDelegate>();
        var exception = new InvalidRefreshTokenException("message");
        next.Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(exception);

        await middleware.InvokeAsync(context, next.Object);

        Assert.Equal(401, context.Response.StatusCode);
    }
}