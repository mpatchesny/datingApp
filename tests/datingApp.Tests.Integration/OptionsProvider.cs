using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace datingApp.Tests.Integration;

public class OptionsProvider
{
    private readonly IConfiguration _configuration;
    public OptionsProvider()
    {
        _configuration = GetConfigurationRoot();
    }

    public T Get<T>(string sectionName) where T : class, new()
    {
        return _configuration.GetOptions<T>(sectionName);
    }

    private static IConfigurationRoot GetConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
    }
}