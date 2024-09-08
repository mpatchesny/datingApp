using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;

namespace datingApp.Infrastructure.Security;

internal sealed class AccessCodeVerificator
{
    public bool Verify(AccessCodeDto accessCode, string userGivenAccessCode, string userEmail)
    {
        bool isCodeValid = accessCode.AccessCode.ToLowerInvariant() == userGivenAccessCode.ToLowerInvariant();
        if (isCodeValid) isCodeValid = isCodeValid && accessCode.EmailOrPhone.ToLowerInvariant() == userEmail.ToLowerInvariant();
        if (isCodeValid) isCodeValid = isCodeValid && accessCode.ExpirationTime >= DateTime.UtcNow;
        return isCodeValid;
    }
}