using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Notifications;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Application.Spatial;
using datingApp.Application.Storage;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Notifications;
using datingApp.Infrastructure.Notifications.Generators;
using datingApp.Infrastructure.Notifications.Services;
using datingApp.Infrastructure.Security;
using datingApp.Infrastructure.Services;
using datingApp.Infrastructure.Spatial;
using datingApp.Infrastructure.Storage;
using FluentStorage;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Scrutor;

namespace datingApp.Infrastructure;

public static class Extensions
{
    private const string PhotoServiceOptionsSectionName = "PhotoService";
    private const string EmailSenderOptionsSectionName = "EmailSender";
    private const string EmailGeneratorOptionsName = "AccessCodeEmail";
    private const string SmsGeneratorOptionsName = "AccessCodeSMS";
    private const string StorageOptionsSectionName = "Storage";
    private const string AppOptionsSectionName = "app";
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var environment = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
        var storagePath = StorageFullPath(environment, configuration);
        ValidateStoragePath(storagePath);
        services.AddSingleton<IBlobStorage>(storage => StorageFactory.Blobs.DirectoryFiles(storagePath));

        services.AddDatabase(configuration);
        services.AddAuth(configuration);
        services.AddHttpContextAccessor();

        services.Configure<AppOptions>(configuration.GetRequiredSection(AppOptionsSectionName));
        services.Configure<PhotoServiceOptions>(configuration.GetRequiredSection(PhotoServiceOptionsSectionName));
        services.Configure<EmailSenderOptions>(configuration.GetRequiredSection(EmailSenderOptionsSectionName));
        services.Configure<EmailGeneratorOptions>(configuration.GetRequiredSection(EmailGeneratorOptionsName));

        services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
        services.AddScoped<IEmailGeneratorFactory, EmailGeneratorFactory>();
        services.AddScoped<INotificationSender<Email>, DummyEmailSender>();

        services.AddScoped<IQueryHandler<GetUpdates, IEnumerable<MatchDto>>, GetUpdatesHandler>(); 
        services.Scan(s => s.FromCallingAssembly()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                .Where(t => !t.Name.Equals("GetUpdatesHandler")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddSingleton<IIsLikedByOtherUserStorage, HttpContextIsLikedByOtherUserStorage>();
        services.AddSingleton<ISpatial, Spatial.Spatial>();

        services.AddScoped<IRevokedRefreshTokensService, RevokedRefreshTokensService>(); 
        services.AddScoped<IDeletedEntityService, DeletedEntityService>(); 
        services.AddScoped<IPhotoValidator, PhotoValidator>(); 
        services.Scan(s => s.FromCallingAssembly()
            .AddClasses(c => c.InNamespaces("datingApp.Infrastructure.Services")
                .Where(c => !c.Name.EndsWith("Options"))
                .Where(c => !c.Name.Equals("RevokedRefreshTokensService"))
                .Where(c => !c.Name.Equals("DeletedEntityService"))
                .Where(c => !c.Name.Equals("FormFilePhotoValidator"))
                .Where(c => !c.Name.Equals("StreamPhotoValidator")))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.AddSingleton<ExceptionMiddleware>();

        return services;
    }

    public static string StorageFullPath(this IWebHostEnvironment environment, IConfiguration configuration)
    {
        var options = configuration.GetOptions<StorageOptions>(StorageOptionsSectionName);
        var storagePath = Path.GetFullPath(String.Format(options.StoragePath, environment.ContentRootPath));
        return storagePath;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);
        return options;
    }

    private static void ValidateStoragePath(string storageFullPath)
    {
        var directoryInfo = new System.IO.DirectoryInfo(storageFullPath);

        if (!directoryInfo.Exists)
        {
            throw new DirectoryNotExistsException($"Storage path: {storageFullPath} not exists.");
        }

        if ((directoryInfo.Attributes & FileAttributes.Directory) == 0)
        {
            throw new NotADirectoryException($"Storage path: {storageFullPath} is not a directory.");
        }

        if ((directoryInfo.Attributes & FileAttributes.ReadOnly) > 0)
        {
            throw new NoWritePrivilegesOnDirectory($"Storage path: {storageFullPath} no write privileges on directory.");
        }
    }
}