using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Application.Exceptions;

public class NoEmailProvidedException : CustomException
{
    public NoEmailProvidedException() : base("No email was provided.")
    {
    }
}