using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Services;

public sealed class PhotoServiceOptions
{
    public int MinPhotoSizeBytes { get; set; }
    public int MaxPhotoSizeBytes { get; set; }
}