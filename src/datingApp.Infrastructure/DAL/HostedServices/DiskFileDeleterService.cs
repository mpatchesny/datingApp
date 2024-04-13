using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Services;

namespace datingApp.Infrastructure.DAL.HostedServices;

public class DiskFileDeleterService : IHostedService
{
    private readonly IServiceProvider _serviceProdivder;
    private readonly FileStorageService _storageService;
    private readonly int _delayInSeconds;
    public DiskFileDeleterService(IServiceProvider serviceProvider)
    {
        _serviceProdivder = serviceProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = _serviceProdivder.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
        dbContext.Database.EnsureCreated();

        while (true)
        {
            // get all physical files
            // look for files in database
            // if not found, delete physical file
            await Task.Delay(_delayInSeconds);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}