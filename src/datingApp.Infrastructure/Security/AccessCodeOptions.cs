using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Security;

public sealed class AccessCodeOptions
{
    public TimeSpan Expiry { get; set; }
}