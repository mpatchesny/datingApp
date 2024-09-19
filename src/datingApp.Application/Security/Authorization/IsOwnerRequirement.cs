using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace datingApp.Application.Security.Authorization;

public class IsOwnerRequirement : IAuthorizationRequirement
{
}