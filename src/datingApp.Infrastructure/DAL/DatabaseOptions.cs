using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL;

internal sealed class DatabaseOptions
{
    public string ConnectionString { get; set; }
    public bool SeedSampleData { get; set; }
}