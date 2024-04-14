using datingApp.Core;
using datingApp.Application;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCore();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors();

builder.Host.UseSerilog((context, logConfig) => logConfig
        .ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
    options => options
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<StorageMiddleware>();

app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.GetFullPath(
                String.Format(builder.Environment.StoragePath(builder.Configuration), builder.Environment.ContentRootPath)
            )
        ),
        RequestPath = "/storage",
    }
);

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
