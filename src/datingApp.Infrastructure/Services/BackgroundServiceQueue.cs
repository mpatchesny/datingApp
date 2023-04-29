using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

public class BackgroundServiceQueue : IBackgroundServiceQueue
{
    private readonly List<dynamic> queue = new List<dynamic>();

    public BackgroundServiceQueue()
    {
    }

    public void Enqueue(dynamic item)
    {
        queue.Add(item);
    }

    public dynamic Dequeue()
    {
        var item = queue.FirstOrDefault();
        if (queue.Count() > 0) queue.RemoveAt(0);
        return item;
    }

}