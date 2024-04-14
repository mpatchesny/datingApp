using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL;

public class ExpiredAccessCodesRemoverOptions
{
    public TimeSpan LoopDelay { get; set; }
}