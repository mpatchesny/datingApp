using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Infrastructure.DAL;

internal static class Extensions
{
    private const string OptionsSectionName = "postgres";
    
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresOptions>(configuration.GetRequiredSection(OptionsSectionName));
        var postgresOptions = configuration.GetOptions<PostgresOptions>(OptionsSectionName);
        services.AddDbContext<DatingAppDbContext>(x => x.UseNpgsql(postgresOptions.ConnectionString));
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        services.AddScoped<IPhotoRepository, PostgresPhotoRepository>();
        services.AddScoped<ISwipeRepository, PostgresSwipeRepository>();
        services.AddScoped<IMatchRepository, PostgresMatchRepository>();
        services.AddScoped<IMessageRepository, PostgresMessageRepository>();
        services.AddHostedService<DatabaseInitializer>();

        // EF Core + Npgsql issue
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return services;
    }
}