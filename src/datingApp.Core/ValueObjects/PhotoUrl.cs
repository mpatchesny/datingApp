using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace datingApp.Core.ValueObjects;

public sealed record PhotoUrl
{
    public string Url { get; }
    public string Extension { get { return GetFileExtensionFromUrl(Url); } }

    public PhotoUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new EmptyPhotoUrlException();
        }
        Url = url;
    }

    // https://stackoverflow.com/questions/23228378/is-there-any-way-to-get-the-file-extension-from-a-url
    private static string GetFileExtensionFromUrl(string url)
    {
        url = url.Split('?')[0];
        url = url.Split('/').Last();
        return url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";
    }

    public static implicit operator string(PhotoUrl url)
        => url.Url;

    public static implicit operator PhotoUrl(string value)
        => new(value);
}