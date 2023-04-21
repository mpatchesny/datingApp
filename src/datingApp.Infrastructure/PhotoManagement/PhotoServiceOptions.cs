using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.PhotoManagement;

public sealed class PhotoServiceOptions
{
    public string StoragePath { get; set; }
    public int MinPhotoSizeBytes { get; set; }
    public int MaxPhotoSizeBytes { get; set; }
}