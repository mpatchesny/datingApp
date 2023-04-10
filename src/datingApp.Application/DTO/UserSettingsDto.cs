using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class UserSettingsDto
{
    public Guid UserId { get; set; }
    public int DiscoverSex { get; set; }
    public int DiscoverAgeFrom { get; set; }
    public int DiscoverAgeTo { get; set; }
    public int DiscoverRange { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}