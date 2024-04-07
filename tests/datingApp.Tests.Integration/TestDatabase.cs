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
        var options = new OptionsProvider().Get<DatabaseOptions>("database");
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