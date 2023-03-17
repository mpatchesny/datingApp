using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class Photo
    {
        public long Id { get; }
        
        [Required(ErrorMessage = "user is required")]
        public User User { get; private set; }

        [Required(ErrorMessage = "path is required")]
        public string Path { get; private set; }

        [Required(ErrorMessage = "ordinal is required")]
        public int Oridinal { get; private set; }

        public Photo(User user, string path, int Oridinal)
        {
            User = user;
            Path = path;
            Oridinal = Oridinal;
        }
    }
}