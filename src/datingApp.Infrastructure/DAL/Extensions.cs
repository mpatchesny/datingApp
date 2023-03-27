using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Infrastructure.DAL;

internal static class Extensions
{
    private const string OptionsSectionName = "postgres";
    
    public static IServiceCollection AddPostgres(this IServiceCollection services)
    {
        var ConnectionString = "Host=localhost;Database=datingapp;Username=postgres;Password=";
        services.AddDbContext<DatingAppDbContext>(x => x.UseNpgsql(ConnectionString));
        // services.AddScoped<IWeeklyParkingSpotRepository, PostgresWeeklyParkingSpotRepository>();
        // services.AddScoped<IUserRepository, PostgresUserRepository>();
        // services.AddHostedService<DatabaseInitializer>();
        // services.AddScoped<IUnitOfWork, PostgresUnitOfWork>();
        // services.TryDecorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));
        // EF Core + Npgsql issue
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return services;
    }
}