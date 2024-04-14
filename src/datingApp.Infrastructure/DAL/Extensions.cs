using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.DAL.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Infrastructure.DAL;

internal static class Extensions
{
    private const string DbOptionsSectionName = "database";
    private const string ExpiredAccessCodesRemoverSectionName = "ExpiredAccessCodesRemover";

    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetRequiredSection(DbOptionsSectionName));
        var postgresOptions = configuration.GetOptions<DatabaseOptions>(DbOptionsSectionName);
        services.AddDbContext<DatingAppDbContext>(x => x.UseNpgsql(postgresOptions.ConnectionString));
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        services.AddScoped<IPhotoRepository, PostgresPhotoRepository>();
        services.AddScoped<ISwipeRepository, PostgresSwipeRepository>();
        services.AddScoped<IMatchRepository, PostgresMatchRepository>();
        services.AddScoped<IMessageRepository, PostgresMessageRepository>();
        services.AddHostedService<DatabaseInitializer>();
        if (postgresOptions.SeedSampleData)
        {
            services.AddHostedService<DatabaseSeeder>();
        }
        services.Configure<ExpiredAccessCodesRemoverOptions>(configuration.GetRequiredSection(ExpiredAccessCodesRemoverSectionName));
        services.AddHostedService<ExpiredAccessCodesRemover>();

        // EF Core + Npgsql issue
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return services;
    }
}