    using System;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace datingApp.Tests.Integration;

internal sealed class TestDatabase : IDisposable
{
    public DatingAppDbContext DbContext { get; }
    public TestDatabase(bool randomDbName = true)
    {
        var options = new OptionsProvider().Get<DatabaseOptions>("database");
        var connString = options.ConnectionString;
        if (randomDbName) connString = String.Format(connString, Guid.NewGuid().ToString());
        DbContext = new DatingAppDbContext(new DbContextOptionsBuilder<DatingAppDbContext>().UseNpgsql(connString).Options);
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}