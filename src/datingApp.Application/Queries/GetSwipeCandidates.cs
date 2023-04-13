using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetSwipeCandidates : IQuery<IEnumerable<PublicUserDto>>
{
    public Guid UserId { get; set; }
    public int AgeFrom { get; set; }
    public int AgeTo { get; set; }
    public int Range { get; set; }
    public int HowMany { get; set; }
    public int Sex { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }

    public GetSwipeCandidates()
    {
    }
    public GetSwipeCandidates(UserSettingsDto settings)
    {
        UserId = settings.UserId;
        AgeFrom = settings.DiscoverAgeFrom;
        AgeTo = settings.DiscoverAgeTo;
        Range = settings.DiscoverRange;
        HowMany = 10;
        Sex = settings.DiscoverSex;
        Lat = settings.Lat; 
        Lon = settings.Lon;
    }
}