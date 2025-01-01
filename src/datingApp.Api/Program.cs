using datingApp.Core;
using datingApp.Application;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using datingApp.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

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

// Enforce HTTPS connection for non-test, non-development environments
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
app.UseAuthentication();
app.UseAuthorization();

// Static files configuration
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

// Swagger configuration
app.Map("swagger/swagger.json", async context =>
{
    var documentationPath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "swagger.json");
    var jsonContent = await File.ReadAllTextAsync(documentationPath);
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(jsonContent);
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("swagger.json", "API Documentation");
});


app.Run();
