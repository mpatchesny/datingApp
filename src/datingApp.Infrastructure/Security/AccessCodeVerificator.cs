using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;

namespace datingApp.Infrastructure.Security;

internal sealed class AccessCodeVerificator : IAccessCodeVerificator
{
    public bool Verify(AccessCodeDto accessCode, string userGivenAccessCode, string userEmail)
    {
        bool isCodeValid = accessCode.AccessCode.ToLowerInvariant() == userGivenAccessCode.ToLowerInvariant();
        if (isCodeValid) isCodeValid = isCodeValid && accessCode.EmailOrPhone.ToLowerInvariant() == userEmail.ToLowerInvariant();
        if (isCodeValid) isCodeValid = isCodeValid && accessCode.ExpirationTime >= DateTime.UtcNow;
        return isCodeValid;
    }
}