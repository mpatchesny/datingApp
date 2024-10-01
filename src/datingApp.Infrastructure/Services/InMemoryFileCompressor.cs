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

        var outputMemoryStream = new MemoryStream();
        using (var inStream = new MemoryStream(compressedFile))
        {
            using (var gzipStream = new GZipStream(inStream, CompressionMode.Decompress))
            {
                CopyStream(gzipStream, outputMemoryStream);
            }
        }
        notCompressedFile = outputMemoryStream.ToArray();
    }

    // https://stackoverflow.com/questions/1354639/writing-to-the-compression-stream-is-not-supported-using-system-io-gzipstream
    private static void CopyStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[8192];
        int bytesRead;
        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }
    }
}
