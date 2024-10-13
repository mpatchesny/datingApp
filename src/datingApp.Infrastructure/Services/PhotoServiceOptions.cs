using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.Services;

public sealed class PhotoServiceOptions
{
    public uint MinPhotoSizeBytes { get; set; }
    public uint MaxPhotoSizeBytes { get; set; }
    public int CompressedImageQuality{ get; set; }
}