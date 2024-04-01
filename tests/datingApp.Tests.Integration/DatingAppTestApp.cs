using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Tests.Integration;

internal sealed class DatingAppTestApp : WebApplicationFactory<Program>
{
    public HttpClient Client { get; }
    public DatingAppTestApp(Action<IServiceCollection> services = null)
    {
        Client = base.WithWebHostBuilder(builder => 
        {
            if (services is not null)
            {
                builder.ConfigureServices(services);
            }
            builder.UseEnvironment("test");
        }).CreateClient();
    }
}