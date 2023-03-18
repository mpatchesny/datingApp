using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class User
    {
        public int Id { get; }
        
        [RegularExpression(@"^[0-9]{9,15}$", ErrorMessage = "phone no can have only numbers")]
        [StringLength(15, ErrorMessage = "phone no must be maximum 15 characters long")]
        public string PhoneNo { get; private set; }

        [EmailAddress]
        public string Email { get; private set; }
        
        [StringLength(15, ErrorMessage = "name must be maximum 15 characters long")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "invalid characters in name")]
        public string Name { get; private set; }
        
        [Range(18, 100, ErrorMessage = "age must be between 18 and 100")]
        public int Age { get; private set; }

        [Range(0, 1, ErrorMessage = "age must be between 18 and 100")]
        public Sex Sex { get; private set; }

        [StringLength(30, ErrorMessage = "job must be maximum 30 characters long")]
        public string? Job { get; private set; }

        [StringLength(400, ErrorMessage = "bio must be maximum 400 characters long")]
        public string? Bio { get; private set; }

        public IEnumerable<Match> Matches => _matches;
        private readonly HashSet<Match> _matches = new();

        public IEnumerable<Photo> Photos => _photos;
        private readonly HashSet<Photo> _photos = new();

        public User(int id, string phoneNo, string email, string name, int age, Sex sex, string? job="", string? bio="")
        {
            Id = id;
            PhoneNo = phoneNo;
            Email = email;
            Name = name;
            Age = age;
            Sex = sex;
            Job = job;
            Bio = bio;
        }

        public bool IsVisible()
        {
            return _photos.Any(x => x.Oridinal == 1);
        }

        public void ChangeAge(int age)
        {
            Age = age;
        }
        public void ChangeBio(string bio)
        {
            Bio = bio;
        }
        public void ChangeJob(string job)
        {
            Job = job;
        }

        public void AddMatch(Match match)
        {
            if (_matches.Any(x => x.Id == match.Id))
            {
                throw new Exception("match already added to the user");
            }
            _matches.Add(match);
        }

        public void RemoveMatch(long matchId)
        {
            _matches.RemoveWhere(x => x.Id == matchId);
        }
        
        public void AddPhoto(Photo photo)
        {
            if (_photos.Any(x => x.Id == photo.Id))
            {
                throw new Exception("photo already added to the user");
            }

            if (_photos.Any(x => x.Oridinal == photo.Oridinal))
            {
                throw new Exception("photo with that oridinal already exists");
            }
            _photos.Add(photo);
        }

        public void RemovePhoto(long photoId)
        {
            _photos.RemoveWhere(x => x.Id == photoId);
        }
    }
}