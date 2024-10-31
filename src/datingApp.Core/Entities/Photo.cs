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
    public PhotoId Id { get; }
    public PhotoUrl Url { get; private set; }
    public Oridinal Oridinal { get; private set; }
    public string Extension { get { return Url.Extension; } }

    private Photo()
    {
        // EF
    }

    public Photo(PhotoId id, PhotoUrl url, Oridinal oridinal)
    {
        Id = id;
        Url = url;
        Oridinal = oridinal;
    }

    public void ChangeOridinal(Oridinal oridinal)
    {
        Oridinal = oridinal;
    }
}