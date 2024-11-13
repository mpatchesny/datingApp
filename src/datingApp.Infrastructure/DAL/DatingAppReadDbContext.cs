using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Infrastructure.DAL;

internal sealed class DatingAppReadDbContext : DbContext
{
    public DbSet<PrivateUserReadModel> Users { get; set; }
    public DbSet<MatchReadModel> Matches { get; set; }
    public DbSet<MessageReadModel> Messages { get; set; }

    public DatingAppReadDbContext(DbContextOptions options) : base(options)
    {
    }
}