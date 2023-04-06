using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Handlers;
using datingApp.Infrastructure.DAL.Repositories;
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
        services.AddScoped<GetMatchesHandler>();
        services.AddScoped<GetMessagesHandler>();
        services.AddScoped<GetPublicUserHandler>();
        services.AddScoped<GetPublicUserHandler>();
        services.AddScoped<GetSwipeCandidatesHandler>();
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