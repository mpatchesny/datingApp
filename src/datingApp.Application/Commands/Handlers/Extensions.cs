using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using Microsoft.Extensions.Caching.Memory;

namespace datingApp.Application.Commands.Handlers;

public static class CacheExtensions
{
    public static void SetCode(this IMemoryCache cache, CodeDto code)
    {
        // FIXME magic number
        cache.Set(code.EmailOrPhone, code, TimeSpan.FromMinutes(15));
    }

    public static CodeDto GetCode(this IMemoryCache cache, string emailOrPhone)
    {
        return cache.Get<CodeDto>(emailOrPhone);
    }
}