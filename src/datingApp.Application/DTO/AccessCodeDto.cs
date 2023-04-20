using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class AccessCodeDto
{
    public string EmailOrPhone { get; set; }
    public string AccessCode { get; set; }
    public DateTime ExpirationTime { get; set; }
    public TimeSpan Expiry { get; set; }
}