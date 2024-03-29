using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace datingApp.Tests.Integration;

internal sealed class DatingAppTestApp : WebApplicationFactory<Program>
{
    public HttpClient Client { get; }
    public DatingAppTestApp()
    {
        Client = base.WithWebHostBuilder(builder => 
        {
            builder.UseEnvironment("test");
        }).CreateClient();
    }
}