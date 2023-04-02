using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Spatial;

public class Spatial : ISpatial
{
    public int CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        throw new NotImplementedException();
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