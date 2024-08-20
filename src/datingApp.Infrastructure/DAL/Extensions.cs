using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.DAL.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using datingApp.Infrastructure.DAL.Options;

namespace datingApp.Infrastructure.DAL;

internal static class Extensions
{
    private const string ConnectionStringsOptionsSectionName = "ConnectionStrings";
    private const string DbOptionsSectionName = "database";
    private const string ExpiredAccessCodesRemoverSectionName = "ExpiredAccessCodesRemover";

    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetRequiredSection(DbOptionsSectionName));
        var connStringOptions = configuration.GetOptions<ConnectionStringsOptions>(ConnectionStringsOptionsSectionName);
        services.AddDbContext<DatingAppDbContext>(x => x.UseNpgsql(connStringOptions.datingApp));
        services.AddScoped<IUserRepository, DbUserRepository>();
        services.AddScoped<IPhotoRepository, DbPhotoRepository>();
        services.AddScoped<ISwipeRepository, DbSwipeRepository>();
        services.AddScoped<IMatchRepository, DbMatchRepository>();
        services.AddScoped<IMessageRepository, DbMessageRepository>();
        services.AddHostedService<DatabaseInitializer>();

        var postgresOptions = configuration.GetOptions<DatabaseOptions>(DbOptionsSectionName);
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