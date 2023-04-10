using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Abstractions;
using datingApp.Application.DTO;

namespace datingApp.Application.Queries;

public class GetPublicUser : IQuery<PublicUserDto>
{
    public Guid UserId { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}