using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL;

internal sealed class PostgresOptions
{
    public string ConnectionString { get; set; }
}