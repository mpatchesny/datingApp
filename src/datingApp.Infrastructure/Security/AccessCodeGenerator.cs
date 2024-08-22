using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using Microsoft.Extensions.Options;

namespace datingApp.Infrastructure.Security;

internal sealed class AccessCodeGenerator : IAccessCodeGenerator
{
    private readonly static Random rand = new Random();
    private readonly string seed = "0123456789QAZXSWEDCVFRTGBNHYUJMKILOP";
    private readonly TimeSpan _expiry;
    public AccessCodeGenerator(IOptions<AccessCodeOptions> options)
    {
        _expiry = options.Value.Expiry;
    }

    public AccessCodeDto GenerateCode(string emailOrPhone)
    {
        string code = Enumerable.Range(0, 6)
            .Select(_ => seed[rand.Next(seed.Length)])
            .ToString();

        return new AccessCodeDto
        { 
            AccessCode = code,
            EmailOrPhone = emailOrPhone,
            Expiry = _expiry,
            ExpirationTime = DateTime.UtcNow + _expiry
        };
    }
}