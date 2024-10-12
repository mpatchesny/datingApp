using datingApp.Core.Repositories;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Repositories;
using datingApp.Infrastructure.DAL.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using datingApp.Infrastructure.DAL.Options;
using datingApp.Application.Repositories;

namespace datingApp.Infrastructure.DAL;

internal static class Extensions
{
    private const string DbOptionsSectionName = "database";
    private const string ConnectionStringsOptionsSectionName = "ConnectionStrings";
    private const string ExpiredAccessCodesRemoverSectionName = "ExpiredDataRemover";

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetRequiredSection(DbOptionsSectionName));
        var connStringOptions = configuration.GetOptions<ConnectionStringsOptions>(ConnectionStringsOptionsSectionName);
        services.AddDbContext<DatingAppDbContext>(x => x.UseNpgsql(connStringOptions.datingApp));

        services.Scan(s => s.FromCallingAssembly()
            .AddClasses(c => c.InNamespaces("datingApp.Application.Repositories"))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddHostedService<DatabaseInitializer>();
        services.Configure<ExpiredAccessCodesRemoverOptions>(configuration.GetRequiredSection(ExpiredAccessCodesRemoverSectionName));
        services.AddHostedService<ExpiredDataRemover>();

        // EF Core + Npgsql issue
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return services;
    }
}