using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using Microsoft.Extensions.Caching.Memory;

namespace datingApp.Infrastructure.Security;

internal sealed class InMemoryCodeStorage : ICodeStorage
{
    private readonly IMemoryCache _cache;
    public InMemoryCodeStorage(IMemoryCache cache)
    {
        _cache = cache;
    }

    public AccessCodeDto Get(string emailOrPhone)
    {
        return _cache.Get<AccessCodeDto>(GetKey(emailOrPhone));
    }

    public void Set(AccessCodeDto code)
    {
        _cache.Set(GetKey(code.EmailOrPhone), code, TimeSpan.FromMinutes(15));
    }

    private string GetKey(string emailOrPhone)
    {
        return $"{emailOrPhone}_access_code";
    }
}