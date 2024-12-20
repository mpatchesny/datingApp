using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Options;

internal sealed class ConnectionStringsOptions
{
    public string ReadWriteDatingApp { get; set; }
    public string ReadOnlyDatingApp { get; set; }
}