using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

namespace datingApp.Tests.Unit.Entities
{
    public class UserLocationTests
    {
        [Theory]
        [InlineData(0.0, -180.1)]
        [InlineData(0.0, 180.1)]
        [InlineData(90.1, 0.0)]
        [InlineData(-90.1, 0.0)]
        public void user_location_should_be_proper_location(double lat, double lon)
        {
            var exception = Record.Exception(() => new Location(lat, lon));
            Assert.NotNull(exception);
            Assert.IsType<InvalidLocationException>(exception);
        }
    }
}