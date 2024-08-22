using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.DAL.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using datingApp.Infrastructure.DAL.Options;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL;

internal static class Extensions
{
    private const string ConnectionStringsOptionsSectionName = "ConnectionStrings";
    private const string DbOptionsSectionName = "database";
    private const string ExpiredAccessCodesRemoverSectionName = "ExpiredAccessCodesRemover";

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetRequiredSection(DbOptionsSectionName));
        var connStringOptions = configuration.GetOptions<ConnectionStringsOptions>(ConnectionStringsOptionsSectionName);
        //services.AddDbContext<DatingAppDbContext>(x => x.UseNpgsql(connStringOptions.datingApp));

        // Replace 'YourDbContext' with the name of your own DbContext derived class.
        var serverVersion = new MySqlServerVersion(new Version(9, 0, 1));
        services.AddDbContext<DatingAppDbContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connStringOptions.datingApp, serverVersion)
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );

        services.AddScoped<IUserRepository, DbUserRepository>();
        services.AddScoped<IPhotoRepository, DbPhotoRepository>();
        services.AddScoped<ISwipeRepository, DbSwipeRepository>();
        services.AddScoped<IMatchRepository, DbMatchRepository>();
        services.AddScoped<IMessageRepository, DbMessageRepository>();
        services.AddHostedService<DatabaseInitializer>();
        services.AddHostedService<DatabaseSeeder>();
        services.Configure<ExpiredAccessCodesRemoverOptions>(configuration.GetRequiredSection(ExpiredAccessCodesRemoverSectionName));
        services.AddHostedService<ExpiredAccessCodesRemover>();

        // EF Core + Npgsql issue
        //ppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return services;
    }
}