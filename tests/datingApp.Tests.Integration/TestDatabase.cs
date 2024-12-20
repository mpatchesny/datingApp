    using System;
using datingApp.Infrastructure;
using datingApp.Infrastructure.DAL;
using datingApp.Infrastructure.DAL.Options;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace datingApp.Tests.Integration;

internal sealed class TestDatabase : IDisposable
{
    public DatingAppDbContext DbContext { get; }
    public ReadOnlyDatingAppDbContext ReadOnlyDbContext { get; }
    private readonly string _connectionString;
    public TestDatabase(bool randomDbName = true)
    {
        var options = new OptionsProvider().Get<ConnectionStringsOptions>("ConnectionStrings");
        _connectionString = options.ReadWriteDatingApp;
        if (randomDbName) _connectionString = string.Format(_connectionString, Guid.NewGuid().ToString());
        DbContext = new DatingAppDbContext(new DbContextOptionsBuilder<DatingAppDbContext>()
            .UseNpgsql(_connectionString).Options);
        ReadOnlyDbContext = new ReadOnlyDatingAppDbContext(new DbContextOptionsBuilder<ReadOnlyDatingAppDbContext>()
            .UseNpgsql(_connectionString).Options);
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}