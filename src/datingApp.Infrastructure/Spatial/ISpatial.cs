using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Spatial;

public interface ISpatial
{
    public int CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    public List<double> GetApproxSquareAroundPoint(double lat, double lon, int distance);
}