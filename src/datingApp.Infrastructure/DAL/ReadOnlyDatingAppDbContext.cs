using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL;

internal sealed class ReadOnlyDatingAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Swipe> Swipes { get; set; }

    public ReadOnlyDatingAppDbContext(DbContextOptions<ReadOnlyDatingAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.LogTo(Console.WriteLine);
    }

    public override int SaveChanges()
    {
        throw new InvalidOperationException("Cannot save changes in read-only db context.");
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new InvalidOperationException("Cannot save changes in read-only db context.");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Cannot save changes in read-only db context.");
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Cannot save changes in read-only db context.");
    }
}