using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class Photo
{
    public Guid Id { get; }
    public Guid UserId { get; private set; }
    public string Url { get; private set; }
    public int Oridinal { get; private set; }
    public PhotoFile File { get; private set; }

    public Photo(Guid id, Guid userId, string url, int oridinal, PhotoFile file = null)
    {
        Id = id;
        UserId = userId;
        SetUrl(url);
        Oridinal = oridinal;
        File = file;
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
            // throw new PhotoEmptyPathException();
        }
        if (Url == url) return;
        Url = url;
    }
}