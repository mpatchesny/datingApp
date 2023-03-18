using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class UserSettings
    {
        public int User { get; private set; }
        public Sex DiscoverSex { get; private set; }
        // [Range(18, 100, ErrorMessage = "discover max age must be between 18 and 100")]
        public int DiscoverMinAge { get; private set; }
        // [Range(18, 100, ErrorMessage = "discover max age must be between 18 and 100")]
        public int DiscoverMaxAge { get; private set; }
        // [Range(1, 100, ErrorMessage = "discover range must be between 1 and 100")]
        public int DiscoverRange { get; private set; }
        public double Lat { get; private set; }
        public double Lon { get; private set; }

        public UserSettings(int user, Sex discoverSex, int discoverMinAge, int discoverMaxAge, int discoverRange, double lat, double lon)
        {
            User = user;
            DiscoverSex = discoverSex;
            DiscoverMaxAge = discoverMaxAge;
            DiscoverMinAge = discoverMinAge;
            DiscoverRange = discoverRange;
            Lat = lat;
            Lon = lon;
        }

        public void ChangeDiscoverSex(Sex sex)
        {
            DiscoverSex = sex;
        }

        public void ChangeDiscoverAge(int minAge, int maxAge)
        {
            DiscoverMinAge = minAge;
            DiscoverMaxAge = maxAge;
        }

        public void ChangeDiscoverRange(int range)
        {
            DiscoverRange = range;
        }

        public void ChangeLocation(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }
    }
}