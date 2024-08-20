using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class NoAccessCodeProvidedException : CustomException
{
    public NoAccessCodeProvidedException() : base("No access code was provided.")
    {
    }
}