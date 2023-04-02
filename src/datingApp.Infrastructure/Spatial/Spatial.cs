using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Spatial;

public class Spatial : ISpatial
{
    private const double r = 6371.009; // kilometers
    private const double toRadMultiplier = Math.PI/180;
    public int CalculateDistance(double fromLat, double fromLon, double toLat, double toLon)
    {
        var fromLatRad = fromLat * toRadMultiplier;
        var fromLonRad = fromLon * toRadMultiplier;
        var toLatRad = toLat * toRadMultiplier;
        var toLonRad = toLon * toRadMultiplier;

        // https://stackoverflow.com/questions/41621957/a-more-efficient-haversine-function
        var sdlat = Math.Sin((toLatRad - fromLatRad) / 2);
        var sdlon = Math.Sin((toLonRad - fromLonRad) / 2);
        var q = sdlat * sdlat + Math.Cos(fromLatRad) * Math.Cos(toLatRad) * sdlon * sdlon;
        var d = 2 * r * Math.Asin(Math.Sqrt(q));
        return (int) Math.Round(d);
    }

    public List<(double lat, double lon)> GetApproxSquareAroundPoint(double lat, double lon, int distance)
    {
        var list = new List<(double lat, double lon)>();
        (double lat, double lon) ne = (lat + distance * 0.009, lon + distance * 0.009);
        (double lat, double lon) nw = (lat - distance * 0.009, lon + distance * 0.009);
        (double lat, double lon) se = (lat + distance * 0.009, lon - distance * 0.009);
        (double lat, double lon) sw = (lat - distance * 0.009, lon - distance * 0.009);
        list.Add(ne);
        list.Add(nw);
        list.Add(se);
        list.Add(sw);
        return list;
    }
}