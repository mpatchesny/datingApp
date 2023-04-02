using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions
{
    public class MatchNotExistsException : CustomException
    {
        public MatchNotExistsException(int matchId) : base($"Match with id {matchId} does not exist")
        {
        }
    }
}