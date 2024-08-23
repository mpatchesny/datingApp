using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public record TokenDto (string Token, DateTime ExpirationTime);

public class JwtDto
{
    public TokenDto AccessToken { get; set; }
    public TokenDto RefreshToken { get; set; }
}