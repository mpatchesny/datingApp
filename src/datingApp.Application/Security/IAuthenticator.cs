using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using datingApp.Application.DTO;

namespace datingApp.Application.Security;

public interface IAuthenticator
{
    ClaimsPrincipal ValidateRefreshToken(string refreshToken);
    JwtDto CreateToken(Guid userId);
}