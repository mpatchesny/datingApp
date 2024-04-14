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

internal sealed class ExpiredAccessCodesRemover : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<ExpiredAccessCodesRemoverOptions> _options;
    public ExpiredAccessCodesRemover(IServiceProvider serviceProvider, IOptions<ExpiredAccessCodesRemoverOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
        dbContext.Database.EnsureCreated();

        var loopDelayInMilliseconds = (int)_options.Value.LoopDelay.TotalMilliseconds;

        while (!stoppingToken.IsCancellationRequested)
        {
            _ = await dbContext.AccessCodes.Where(x => x.ExpirationTime < DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken: stoppingToken);
            await Task.Delay(loopDelayInMilliseconds, stoppingToken);
        }
    }
}