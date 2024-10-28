using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class Photo
{
    public Guid Id { get; }
    public UserId UserId { get; private set; }
    public string Url { get; private set; }
    public int Oridinal { get; private set; }
    public string Extension { get { return GetFileExtensionFromUrl(Url); } }
    private Photo()
    {
        // EF
    }
    public Photo(Guid id, UserId userId, string url, int oridinal)
    {
        Id = id;
        UserId = userId;
        SetUrl(url);
        Oridinal = oridinal;
    }

    public void ChangeOridinal(int oridinal)
    {
        if (oridinal < -1)
        {
            oridinal = 0;
        }
        if (Oridinal == oridinal) return;
        Oridinal = oridinal;
    }

    private void SetUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new EmptyPhotoUrlException();
        }
        if (Url == url) return;
        Url = url;
    }

    // https://stackoverflow.com/questions/23228378/is-there-any-way-to-get-the-file-extension-from-a-url
    private static string GetFileExtensionFromUrl(string url)
    {
        url = url.Split('?')[0];
        url = url.Split('/').Last();
        return url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";
}
}