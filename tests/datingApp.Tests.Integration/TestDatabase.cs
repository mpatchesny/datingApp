    using System;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace datingApp.Tests.Integration;

internal sealed class TestDatabase : IDisposable
{
    public DatingAppDbContext DbContext { get; }
    public TestDatabase()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .Build();
        
        string optionsSectionName = "database";
        var options = configuration.GetOptions<DatabaseOptions>(optionsSectionName);
        DbContext = new DatingAppDbContext(new DbContextOptionsBuilder<DatingAppDbContext>().UseNpgsql(options.ConnectionString).Options);
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}