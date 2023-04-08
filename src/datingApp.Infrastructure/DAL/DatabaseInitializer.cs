using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace datingApp.Infrastructure.DAL
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
            using var scope = _serviceProdivder.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);

            if (dbContext.Users.Count() < 3)
            {
                var settings = new UserSettings(0, Sex.Female, 18, 30, 100, 0.0, 0.0);
                var testUser = new User(0, "012345678", "test@test.com", "janusz", new DateOnly(1999,1,1), Sex.Male, null, settings);
                var settings2 = new UserSettings(0, Sex.Male, 18, 30, 100, 0.0, 0.0);
                var testUser2 = new User(0, "111111111", "test1@test.com", "grazyna", new DateOnly(1999,1,1), Sex.Female, null, settings2);
                var settings3 = new UserSettings(0, Sex.Male, 18, 30, 100, 0.0, 0.0);
                var testUser3 = new User(0, "222222222", "test2@test.com", "karyna", new DateOnly(1999,1,1), Sex.Female, null, settings3);
                dbContext.Users.Add(testUser);
                dbContext.Users.Add(testUser2);
                dbContext.Users.Add(testUser3);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Photos.Count() == 0)
            {
                var photo = new Photo(0, 1, "abc", 0);
                dbContext.Photos.Add(photo);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Matches.Count() == 0)
            {
                var match = new Match(0, 1, 2, false, false, null, DateTime.UtcNow);
                dbContext.Matches.Add(match);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Messages.Count() == 0)
            {
                var message = new Message(0, 1, 1, "hej :)", false, DateTime.UtcNow);
                dbContext.Messages.Add(message);
                await dbContext.SaveChangesAsync();
            };
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}