using datingApp.Core;
using datingApp.Application;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using datingApp.Infrastructure.Storage;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCore();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors();

builder.Host.UseSerilog(
    (context, logConfig) => logConfig.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// enforce HTTPS connection for  non-test, non-development environments
if (!app.Environment.IsEnvironment("test") && !app.Environment.IsEnvironment("development"))
{
    app.Use(
        async (context, next) =>
        {
            if (!context.Request.IsHttps)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Bad Request: only HTTPS is allowed.");
            }
            else
            {
                await next();
            }
        }
    );
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-8.0&tabs=visual-studio%2Clinux-ubuntu#http-redirection-to-https-causes-err_invalid_redirect-on-the-cors-preflight-request
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var storagePath = builder.Environment.StorageFullPath(builder.Configuration);

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(storagePath),
        RequestPath = "/storage",
    }
);

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.MapGet("api", (IOptions<AppOptions> options) => Results.Ok(options.Value.Name));

app.Run();
