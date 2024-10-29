using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.ValueObjects;

public sealed record Location
{
    public double Lat { get; private set; }
    public double Lon { get; private set; }

    private Location()
    {
        // EF
    }

    public Location(double lat, double lon)
    {
        if (lat > 90 | lat < -90 |
            lon > 180 | lon < -180)
        {
            throw new InvalidLocationException();
        }
        Lat = lat;
        Lon = lon;
    }

    public override string ToString() => $"{Lat}, {Lon}";
}