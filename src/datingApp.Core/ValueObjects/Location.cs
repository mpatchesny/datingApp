using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.ValueObjects;

public sealed record Location
{
    public double Lat { get; }
    public double Lon { get; }

    public Location(double lat, double lon)
    {
        Lat = lat;
        Lon = lon;
    }
}