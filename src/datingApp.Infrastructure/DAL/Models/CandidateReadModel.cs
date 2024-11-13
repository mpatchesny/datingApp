using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal sealed class CandidateReadModel
{
    public PublicUserReadModel User { get; set; }
    public int LikesCount { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int Sex { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}