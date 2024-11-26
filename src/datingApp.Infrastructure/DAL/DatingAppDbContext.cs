using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure;

internal sealed class DatingAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Swipe> Swipes { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<AccessCodeDto> AccessCodes { get; set; }
    public DbSet<DeletedEntityDto> DeletedEntities { get; set; }
    public DbSet<TokenDto> RevokedRefreshTokens { get; set; }

    public DatingAppDbContext(DbContextOptions<DatingAppDbContext> options) : base(options)
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
}