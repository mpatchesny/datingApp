using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.DAL.BackgroundServices;

internal sealed class ExpiredDataRemover : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _loopDelay;
    public ExpiredDataRemover(IServiceProvider serviceProvider, IOptions<ExpiredAccessCodesRemoverOptions> options)
    {
        _serviceProvider = serviceProvider;
        _loopDelay = options.Value.LoopDelay;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
        dbContext.Database.EnsureCreated();

        while (!stoppingToken.IsCancellationRequested)
        {
            dbContext.AccessCodes.RemoveRange(dbContext.AccessCodes.Where(x => x.ExpirationTime < DateTime.UtcNow));
            dbContext.RevokedRefreshTokens.RemoveRange(dbContext.RevokedRefreshTokens.Where(x => x.ExpirationTime < DateTime.UtcNow));
            _ = await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(_loopDelay, stoppingToken);
        }
    }
}