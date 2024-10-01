using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.Services;

public interface IFileCompressor
{
    public void Compress(byte[] notCompressedFile, out byte[] compressedFile);
    public void Decompress(byte[] compressedFile, out byte[] notCompressedFile);
}