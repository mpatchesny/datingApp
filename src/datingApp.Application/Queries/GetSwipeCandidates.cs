using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetSwipeCandidates : IQuery<IEnumerable<PublicUserDto>>
{
    public int UserId { get; set; }
    public int AgeFrom { get; set; }
    public int AgeTo { get; set; }
    public int Range { get; set; }
    public int HowMany { get; set; }
    public int Sex { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}