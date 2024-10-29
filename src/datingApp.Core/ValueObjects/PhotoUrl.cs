using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace datingApp.Core.ValueObjects;

public sealed record PhotoUrl
{
    public string Value { get; }
    public string Extension { get { return GetFileExtensionFromUrl(Value); } }

    public PhotoUrl(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new EmptyPhotoUrlException();
        }
        Value = value;
    }

    // https://stackoverflow.com/questions/23228378/is-there-any-way-to-get-the-file-extension-from-a-url
    private static string GetFileExtensionFromUrl(string url)
    {
        url = url.Split('?')[0];
        url = url.Split('/').Last();
        return url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";
    }

    public static implicit operator string(PhotoUrl url)
        => url.Value;

    public static implicit operator PhotoUrl(string value)
        => new(value);

    public override string ToString() => Value;
}