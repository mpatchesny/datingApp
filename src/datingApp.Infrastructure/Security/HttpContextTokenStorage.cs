using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;

namespace datingApp.Infrastructure.Security;

internal sealed class HttpContextTokenStorage : ITokenStorage
{
    private const string JwtDtoKey = "jwt";
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpContextTokenStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public JwtDto Get()
    {
        return _httpContextAccessor.HttpContext?.Items.TryGetValue(JwtDtoKey, out var jwt) == true
            ? jwt as JwtDto
            : null;
    }

    public void Set(JwtDto jwt)
    {
        _httpContextAccessor.HttpContext?.Items.TryAdd(JwtDtoKey, jwt);
    }
}