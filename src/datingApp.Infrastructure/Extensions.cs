using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.PhotoManagement;
using datingApp.Application.Queries;
using datingApp.Application.Security;
using datingApp.Application.Services;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Middleware;
using datingApp.Infrastructure.PhotoManagement;
using datingApp.Infrastructure.Security;
using datingApp.Infrastructure.Services;
using datingApp.Infrastructure.Spatial;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("MySpot.Tests.Integration")]
namespace datingApp.Infrastructure;

public static class Extensions
{
    private const string EmailSenderOptionsSectionName = "EmailSender";
    private const string PhotoServiceOptionsSectionName = "PhotoService";
    private const string StorageOptionsSectionName = "Storage";
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        services.AddAuth(configuration);
        services.AddHttpContextAccessor();
        services.Configure<EmailSenderOptions>(configuration.GetRequiredSection(EmailSenderOptionsSectionName));
        services.Configure<PhotoServiceOptions>(configuration.GetRequiredSection(PhotoServiceOptionsSectionName));
        services.Configure<StorageOptions>(configuration.GetRequiredSection(StorageOptionsSectionName));
        services.AddSingleton<ISpatial, Spatial.Spatial>();
        services.AddScoped<IQueryHandler<GetMatches, PaginatedDataDto>, GetMatchesHandler>();
        services.AddScoped<IQueryHandler<GetMessages, PaginatedDataDto>, GetMessagesHandler>();
        services.AddScoped<IQueryHandler<GetPhoto, PhotoDto>, GetPhotoHandler>();
        services.AddScoped<IQueryHandler<GetMessage, MessageDto>, GetMessageHandler>();
        services.AddScoped<IQueryHandler<GetIsLikedByOtherUser, IsLikedByOtherUserDto>, GetIsLikedByOtherUserHandler>();
        services.AddScoped<IQueryHandler<GetPublicUser, PublicUserDto>, GetPublicUserHandler>();
        services.AddScoped<IQueryHandler<GetPrivateUser, PrivateUserDto>, GetPrivateUserHandler>();
        services.AddScoped<IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>>, GetSwipeCandidatesHandler>();
        services.AddScoped<IQueryHandler<GetUpdates, IEnumerable<MatchDto>>, GetUpdatesHandler>();
        services.AddSingleton<IEmailSender, DummyEmailSender>();
        services.AddSingleton<IPhotoService, PhotoService>();
        services.AddSingleton<IFileStorage, FileStorage>();
        services.AddSingleton<ExceptionMiddleware>();
        services.AddSingleton<OptionsMiddleware>();
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