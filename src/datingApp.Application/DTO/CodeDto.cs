using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class AccessCodeDto
{
    public string EmailOrPhone { get; }
    public string AccessCode { get; }
    public TimeSpan Expiry { get; }
}