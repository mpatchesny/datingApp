using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class Photo
{
    public Guid Id { get; }
    public Guid UserId { get; private set; }
    public string Url { get; private set; }
    public int Oridinal { get; private set; }
    public string Extension
    {
        get
        {
            var x = Url.Split(".");
            return x.Length > 0 ? x[^1] : "";
        }
    }
    private Photo()
    {
        // EF
    }
    public Photo(Guid id, Guid userId, string url, int oridinal)
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
}