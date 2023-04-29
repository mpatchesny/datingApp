using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Services;

public sealed class BackgroundServiceOptions
{
    public int DelayBetweenExecutions { get; set; }
}