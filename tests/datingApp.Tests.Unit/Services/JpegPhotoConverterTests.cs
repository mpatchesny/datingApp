using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace datingApp.Tests.Unit.Services;

public class JpegPhotoConverterTests
{
    [Fact]
    public void Test1()
    {

        Assert.True(true);
    }

    private readonly IOptions<PhotoServiceOptions> _options;
    public JpegPhotoConverterTests()
    {
        _options = Options.Create<PhotoServiceOptions>(new PhotoServiceOptions());
        _options.Value.MinPhotoSizeBytes = 1;
        _options.Value.MaxPhotoSizeBytes = 999999;
    }
}