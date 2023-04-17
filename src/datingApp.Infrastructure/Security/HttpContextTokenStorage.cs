using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;

namespace datingApp.Infrastructure.Security;

public class HttpContextTokenStorage : ITokenStorage
{
    private const string JwtDtoKey = "jwt";
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpContextTokenStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public JwtDto Get()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return null;
        }

        if (_httpContextAccessor.HttpContext.Items.TryGetValue(JwtDtoKey, out var jwt))
        {
            return jwt as JwtDto;
        }

        return null;
    }

    public void Set(JwtDto jwt)
    {
        _httpContextAccessor.HttpContext?.Items.TryAdd(JwtDtoKey, jwt);
    }
}