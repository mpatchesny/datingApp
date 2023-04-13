using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;
using datingApp.Application.Queries;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.Exceptions;
using datingApp.Infrastructure.Spatial;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("MySpot.Tests.Integration")]
namespace datingApp.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        services.AddSingleton<ISpatial, Spatial.Spatial>();
        services.AddScoped<IQueryHandler<GetMatches, IEnumerable<MatchDto>>, GetMatchesHandler >();
        services.AddScoped<IQueryHandler<GetPhoto, PhotoDto>, GetPhotoHandler >();
        services.AddScoped<IQueryHandler<GetMessages, IEnumerable<MessageDto>>, GetMessagesHandler >();
        services.AddScoped<IQueryHandler<GetMessage, MessageDto>, GetMessageHandler >();
        services.AddScoped<IQueryHandler<GetMatch, IsMatchDto>, GetMatchHandler >();
        services.AddScoped<IQueryHandler<GetPublicUser, PublicUserDto>, GetPublicUserHandler >();
        services.AddScoped<IQueryHandler<GetPrivateUser, PrivateUserDto>, GetPrivateUserHandler >();
        services.AddScoped<IQueryHandler<GetSwipeCandidates, IEnumerable<PublicUserDto>>, GetSwipeCandidatesHandler >();
        services.AddSingleton<ExceptionMiddleware>();
        return services;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);
        return options;
    }
}