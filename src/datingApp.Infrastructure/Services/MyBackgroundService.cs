using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

// https://zakodowane.net/zadania-w-tle-backgroundservice-vs-ihostedservice-w-asp-net-core/
public class MyBackgroundService : BackgroundService
{
    private readonly ILogger<MyBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<BackgroundServiceOptions> _backgroundServiceOptions;
    public MyBackgroundService(IServiceProvider serviceProvider,
                               ILogger<MyBackgroundService> logger,
                               IOptions<BackgroundServiceOptions> backgroundServiceOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _backgroundServiceOptions = backgroundServiceOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                // TODO
                // wykonywanie zapytania
            }
            catch (Exception ex)
            {
                var error = $"{nameof(MyBackgroundService)}: error during processing.";
                _logger.LogWarning(ex, error);
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(_backgroundServiceOptions.Value.DelayBetweenExecutions));
            }
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(MyBackgroundService)}: service is starting.");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(MyBackgroundService)}: service is stopping.");
        return base.StopAsync(cancellationToken);
    }
}