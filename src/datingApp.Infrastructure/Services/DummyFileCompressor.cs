using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

internal sealed class DummyFileCompressor : IFileCompressor
{
    public void Compress(byte[] notCompressedFile, out byte[] compressedFile)
    {
        compressedFile = notCompressedFile;
    }

    public void Decompress(byte[] compressedFile, out byte[] notCompressedFile)
    {
        notCompressedFile = compressedFile;
    }
}