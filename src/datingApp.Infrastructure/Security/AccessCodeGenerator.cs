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
        char[] code = new char[6];
        for (int i = 0; i < code.Length; i++)
        {
            var randInt = rand.Next(0, seed.Length);
            code[i] = seed[randInt];
        }

        return new AccessCodeDto
        { 
            AccessCode = new string(code),
            EmailOrPhone = emailOrPhone,
            Expiry = _expiry,
            ExpirationTime = DateTime.UtcNow + _expiry
        };
    }
}