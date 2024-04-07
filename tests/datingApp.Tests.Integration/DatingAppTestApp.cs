using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using datingApp.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Tests.Integration;

internal sealed class DatingAppTestApp : WebApplicationFactory<Program>
{
    public HttpClient Client { get; }
    public DatingAppTestApp(Action<IServiceCollection> services = null)
    {
        Client = base.WithWebHostBuilder(builder => 
        {
            builder.UseEnvironment("test");
            var tempFolder = Path.GetTempPath();
            if (services is not null)
            {
                builder.UseSetting("Storage:StoragePath", tempFolder);
            }
        }).CreateClient();
    }
}