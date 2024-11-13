using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal sealed class UserSettingsReadModel
{
    public int PreferredSex { get; set; }
    public int PreferredAgeFrom { get; set; }
    public int PreferredAgeTo { get; set; }
    public int PreferredMaxDistance { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}