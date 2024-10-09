using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Notifications;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Queries;
using datingApp.Application.Repositories;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Application.Storage;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Notifications;
using datingApp.Infrastructure.Security;
using datingApp.Infrastructure.Services;
using datingApp.Infrastructure.Spatial;
using datingApp.Infrastructure.Storage;
using FluentStorage;
using FluentStorage.Blobs;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace datingApp.Infrastructure;

public static class Extensions
{
    private const string EmailSenderOptionsSectionName = "EmailSender";
    private const string StorageOptionsSectionName = "Storage";
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddAuth(configuration);
        services.AddHttpContextAccessor();
        services.Configure<EmailSenderOptions>(configuration.GetRequiredSection(EmailSenderOptionsSectionName));
        services.Configure<StorageOptions>(configuration.GetRequiredSection(StorageOptionsSectionName));

        services.AddScoped<IQueryHandler<GetUpdates, IEnumerable<MatchDto>>, GetUpdatesHandler>();
        services.Scan(s => s.FromCallingAssembly()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>))
                .Where(t => !t.Name.Equals("GetUpdatesHandler")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        services.AddScoped<IDeletedEntityRepository, DbDeletedEntityRepository>();

        services.AddSingleton<INotificationSender<Email>, DummyEmailSender>();
        services.AddSingleton<IIsLikedByOtherUserStorage, HttpContextIsLikedByOtherUserStorage>();
        services.AddSingleton<ISpatial, Spatial.Spatial>();
        services.Scan(s => s.FromCallingAssembly()
            .AddClasses(c => c.InNamespaces("datingApp.Infrastructure.Services")
                .Where(t => !t.Name.Contains("Dummy") && !t.Name.Contains("Option")))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
        // services.AddSingleton<IFileStorageService, FileStorageService>();
        // services.AddSingleton<IPhotoService, PhotoService>();
        // services.AddSingleton<IPhotoOrderer, PhotoOrderer>();
        services.AddSingleton<ExceptionMiddleware>();
        services.AddSingleton<IBlobStorage>(storage => StorageFactory.Blobs.FromConnectionString($"disk://path={StoragePath}"));
        return services;
    }

    public static string StoragePath(this IWebHostEnvironment environment, IConfiguration configuration)
    {
        var options = configuration.GetOptions<StorageOptions>(StorageOptionsSectionName);
        return options.StoragePath;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);
        return options;
    }
}