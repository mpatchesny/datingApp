using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class Photo
    {
        public int Id { get; }
        public int UserId { get; private set; }
        public string Path { get; private set; }
        public int Oridinal { get; private set; }

        public Photo(int id, int userId, string path, int oridinal)
        {
            Id = id;
            UserId = userId;
            Path = path;
            Oridinal = oridinal;
        }
    }
}