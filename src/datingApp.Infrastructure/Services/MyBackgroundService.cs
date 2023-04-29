using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Services;

// https://zakodowane.net/zadania-w-tle-backgroundservice-vs-ihostedservice-w-asp-net-core/
public class MyBackgroundService : BackgroundService
{
    private readonly ILogger<MyBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<BackgroundServiceOptions> _backgroundServiceOptions;
    private readonly IBackgroundServiceQueue _queue;
    public MyBackgroundService(IServiceProvider serviceProvider,
                               ILogger<MyBackgroundService> logger,
                               IOptions<BackgroundServiceOptions> backgroundServiceOptions,
                               IBackgroundServiceQueue queue)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _backgroundServiceOptions = backgroundServiceOptions;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
                    var item = _queue.Dequeue();
                    if (item != null)
                    {
                        // TODO
                        // parse json to command
                        // handle command
                        
                    }
                };
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