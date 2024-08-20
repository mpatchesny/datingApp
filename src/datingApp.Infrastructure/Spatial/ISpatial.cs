using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Spatial;

public struct Coords
{
    public double NorthLat { get; }
    public double SouthLat { get; }
    public double EastLon { get; }
    public double WestLon { get; }
    public Coords(double northLat, double southLat, double eastLon, double westLon)
    {
        NorthLat = northLat;
        SouthLat = southLat;
        EastLon = eastLon;
        WestLon = westLon;
    }
}

public interface ISpatial
{
    public int CalculateDistanceInKms(double lat1, double lon1, double lat2, double lon2);
    public Coords GetApproxSquareAroundPoint(double lat, double lon, int distance);
}