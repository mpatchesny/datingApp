using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class Photo
    {
        public int Id { get; }
        public int User { get; private set; }
        public string Path { get; private set; }
        public int Oridinal { get; private set; }

        public Photo(int id, int user, string path, int oridinal)
        {
            Id = id;
            User = user;
            Path = path;
            Oridinal = oridinal;
        }
    }
}