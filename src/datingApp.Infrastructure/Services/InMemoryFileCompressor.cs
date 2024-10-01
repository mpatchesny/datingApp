using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using datingApp.Application.Services;

namespace datingApp.Infrastructure.Services;

internal sealed class InMemoryFileCompressor : IFileCompressor
{
    public void Compress(byte[] notCompressedFile, out byte[] compressedFile)
    {
        if (notCompressedFile == null)
        {
            compressedFile = null;
            return;
        }

        using (var outputMemoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputMemoryStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(notCompressedFile, 0, notCompressedFile.Length);
            }
            compressedFile = outputMemoryStream.ToArray();
        }
    }

    public void Decompress(byte[] compressedFile, out byte[] notCompressedFile)
    {
        if (compressedFile == null)
        {
            notCompressedFile = null;
            return;
        }

        using (var outputMemoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputMemoryStream, CompressionMode.Decompress))
            {
                gzipStream.Write(compressedFile, 0, compressedFile.Length);
            }
            notCompressedFile = outputMemoryStream.ToArray();
        }
    }
}