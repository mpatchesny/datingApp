using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;

namespace datingApp.Application.Security;

public interface IAccessCodeVerificator
{
    public bool Verify(AccessCodeDto accessCode, string userGivenAccessCode, string userEmail);
}