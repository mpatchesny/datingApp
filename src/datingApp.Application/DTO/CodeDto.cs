using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class CodeDto
{
    public string EmailOrPhone { get; set; }
    public string Code { get; set; }
    public TimeSpan Expiry { get; set; }
}