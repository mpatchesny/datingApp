using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;

namespace datingApp.Tests.Integration.Controllers;

internal sealed class DatingAppTestApp : WebApplicationFactory<Program>
{
    public DatingAppTestApp()
    {
        // pass
    }
}