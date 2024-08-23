using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Security;

public sealed class AuthOptions
{
    public sealed class Token
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }
        public TimeSpan Expiry { get; set; }
    }
    public Token AccessToken { get; set; }
    public Token RefreshToken { get; set; }
}