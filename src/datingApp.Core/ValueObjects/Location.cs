using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects
{
    public record Location
    {
         public double Lat { get; set; }
         public double Lon { get; set; }
    }
}