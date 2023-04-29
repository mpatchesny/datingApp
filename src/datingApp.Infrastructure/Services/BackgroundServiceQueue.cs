using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

internal sealed class BackgroundServiceQueue : IBackgroundServiceQueue
{
    private readonly DatingAppDbContext _dbContext;

    public BackgroundServiceQueue(DatingAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Enqueue(dynamic item)
    {
        string json = JsonSerializer.Serialize(item);
        _dbContext.Queue.AddAsync(json);
        _dbContext.SaveChangesAsync();
    }

    public dynamic Dequeue()
    {
        var item = _dbContext.Queue.FirstOrDefault();
        _dbContext.Queue.Remove(item);
        _dbContext.SaveChangesAsync();
        return item;
    }

}