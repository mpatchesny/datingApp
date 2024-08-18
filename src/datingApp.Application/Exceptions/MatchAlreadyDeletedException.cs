using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class MatchAlreadyDeletedException : CustomException
{
    public MatchAlreadyDeletedException(Guid matchId) :base($"Match {matchId} is deleted permanently.")
    {
    }
}