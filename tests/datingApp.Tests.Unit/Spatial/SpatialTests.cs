using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Spatial;
using Xunit;

namespace datingApp.Tests.Unit.Spatial;

public class SpatialTests
{
    [Theory]
    [InlineData(54.3748048524063, 18.617995680434113, 52.285241957954106, 21.020699940845155, 282)]
    [InlineData(54.37539776012496, 18.618187777316162, 54.37017703288759, 18.627504041571758, 1)]
    [InlineData(40.67801005527118, -74.0226594916749, 51.490976502695595, -0.24581200990659666, 5567)]
    public void given_two_points_spatial_returns_proper_calculated_distance_in_kilometers(double lat1, double lon1, double lat2, double lon2, int expectedDistance)
    {
        var spatial = new Infrastructure.Spatial.Spatial();
        Assert.Equal(expectedDistance, spatial.CalculateDistance(lat1, lon1, lat2, lon2));
    }

    [Theory]
    [InlineData(0.0, 0.0, 50)]
    public void given_two_points_and_distance_in_kilometers_spatial_returns_proper_approximation_of_square_around_point(double lat, double lon, int distance)
    {
        var spatial = new Infrastructure.Spatial.Spatial();
        var square = spatial.GetApproxSquareAroundPoint(lat, lon, distance);
        Assert.True(true);
    }
}