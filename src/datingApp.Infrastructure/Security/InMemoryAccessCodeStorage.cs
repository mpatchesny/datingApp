using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using Microsoft.Extensions.Caching.Memory;

namespace datingApp.Infrastructure.Security;

internal sealed class InMemoryAccessCodeStorage : IAccessCodeStorage
{
    private readonly IMemoryCache _cache;
    public InMemoryAccessCodeStorage(IMemoryCache cache)
    {
        _cache = cache;
    }

    public AccessCodeDto Get(string emailOrPhone)
    {
        var key = GetKey(emailOrPhone);
        var code = _cache.Get<AccessCodeDto>(key);
        _cache.Remove(key);
        return code;
    }

    public void Set(AccessCodeDto code)
    {
        _cache.Set(GetKey(code.EmailOrPhone), code, code.Expiry);
    }

    private string GetKey(string emailOrPhone)
    {
        return $"{emailOrPhone}_access_code";
    }
}