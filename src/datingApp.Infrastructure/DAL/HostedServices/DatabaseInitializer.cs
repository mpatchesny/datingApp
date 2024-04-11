using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace datingApp.Infrastructure.DAL.HostedServices
{
    internal sealed class DatabaseInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProdivder;
        public DatabaseInitializer(IServiceProvider serviceProvider)
        {
            _serviceProdivder = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProdivder.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}