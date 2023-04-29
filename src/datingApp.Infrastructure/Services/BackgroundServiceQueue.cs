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
        QueueItem queueItem = new QueueItem();
        queueItem.Data = json;
        _dbContext.Queue.Add(queueItem);
        _dbContext.SaveChanges();
    }

    public dynamic Dequeue()
    {
        var item = _dbContext.Queue.FirstOrDefault();
        _dbContext.Queue.Remove(item);
        _dbContext.SaveChanges();
        return item;
    }

}