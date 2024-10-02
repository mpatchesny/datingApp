using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure.Services;
using Xunit;

namespace datingApp.Tests.Integration.Services;

public class InMemoryFileCompressorTests
{
    [Fact]
    public void compress_compresses_file()
    {
        var compressor = new InMemoryFileCompressor();
        byte[] inputFile = Enumerable.Repeat((byte) byte.MaxValue, 1000 * 1000).ToArray();
        compressor.Compress(inputFile, out byte[] outputFile);
        Assert.True(inputFile.Length > outputFile.Length);
    }

    [Fact]
    public void given_notCompressedFile_passed_to_compress_is_null_compressedFile_out_is_null()
    {
        var compressor = new InMemoryFileCompressor();
        byte[] inputFile = null;
        byte[] outputFile = null;
        compressor.Compress(inputFile, out outputFile);
        Assert.Null(outputFile);
    }

    [Fact]
    public void decompress_decompresses_file()
    {
        var compressor = new InMemoryFileCompressor();
        byte[] inputFile = new byte[]{
                0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0xFB, 0xFF,
                0x9F, 0xF6, 0x00, 0x00, 0x81, 0x86, 0xD2, 0x03, 0x64, 0x00, 0x00, 0x00
            };
        compressor.Decompress(inputFile, out byte[] outputFile);
        Assert.True(inputFile.Length < outputFile.Length);
    }

    [Fact]
    public void given_compressedFile_passed_to_decompress_is_null_notCompressedFile_out_is_null()
    {
        var compressor = new InMemoryFileCompressor();
        byte[] inputFile = null;
        byte[] outputFile = null;
        compressor.Decompress(inputFile, out outputFile);
        Assert.Null(outputFile);
    }
}