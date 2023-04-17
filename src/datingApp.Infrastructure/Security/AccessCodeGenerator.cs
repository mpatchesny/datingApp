using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;

namespace datingApp.Infrastructure.Security;

internal sealed class AccessCodeGenerator : IAccessCodeGenerator
{
    private readonly static Random rand = new Random();
    private readonly string seed = "0123456789QAZXSWEDCVFRTGBNHYUJMKILOP";
    public AccessCodeGenerator()
    {
        // todo: options
    }
    public AccessCodeDto GenerateCode(string emailOrPhone)
    {
        char[] code = {};
        for (int i = 0; i < 6; i++)
        {
            var randInt = rand.Next(0, seed.Length);
            code[i] = seed[randInt];
        }

        return new AccessCodeDto
        { 
            AccessCode = code.ToString(),
            EmailOrPhone = emailOrPhone,
            Expiry = TimeSpan.FromMinutes(15),
            ExpirationTime = DateTime.UtcNow + TimeSpan.FromMinutes(15)
        };
    }
}