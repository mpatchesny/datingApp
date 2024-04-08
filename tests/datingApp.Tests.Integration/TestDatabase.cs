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
        string stringSup = randomDbName ? Guid.NewGuid().ToString() : "";
        var connString = options.ConnectionString;
        connString = String.Format(connString, stringSup);
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