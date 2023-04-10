using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class Photo
{
    public int Id { get; }
    public Guid UserId { get; private set; }
    public string Path { get; private set; }
    public int Oridinal { get; private set; }

    public Photo(int id, Guid userId, string path, int oridinal)
    {
        Id = id;
        UserId = userId;
        SetPath(path);
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

    private void SetPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new PhotoEmptyPathException();
        }
        if (Path == path) return;
        Path = path;
    }

}