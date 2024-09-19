    using System;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace datingApp.Tests.Integration;

internal sealed class TestDatabase : IDisposable
{
    public DatingAppDbContext DbContext { get; }
    public TestDatabase(bool randomDbName = true)
    {
        var options = new OptionsProvider().Get<ConnectionStringsOptions>("ConnectionStrings");
        var connString = options.datingApp;
        if (randomDbName) connString = string.Format(connString, Guid.NewGuid().ToString());
        DbContext = new DatingAppDbContext(new DbContextOptionsBuilder<DatingAppDbContext>().UseNpgsql(connString).Options);
        var databaseName = DbContext.Database.GetDbConnection().Database;
        DbContext.Database.ExecuteSqlRaw($"DROP DATABASE IF EXISTS {databaseName}");
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}