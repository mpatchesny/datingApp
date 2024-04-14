using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.DAL.HostedServices;

public class ExpiredAccessCodesRemover : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<ExpiredAccessCodesRemoverOptions> _options;
    public ExpiredAccessCodesRemover(IServiceProvider serviceProvider, IOptions<ExpiredAccessCodesRemoverOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
        dbContext.Database.EnsureCreated();

        var loopDelayInMilliseconds = (int)_options.Value.LoopDelay.TotalMilliseconds;

        while (true)
        {
            if ((cancellationToken.IsCancellationRequested)) return;

            await dbContext.AccessCodes.Where(x => x.ExpirationTime < DateTime.UtcNow).ExecuteDeleteAsync();
            await Task.Delay(loopDelayInMilliseconds);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}