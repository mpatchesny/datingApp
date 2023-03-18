using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class User
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex PhoneRegex = new Regex(@"^[0-9]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex NameRegex = new Regex(@"^[a-zA-Z''-'\s]$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public int Id { get; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public Sex Sex { get; private set; }
        public string Job { get; private set; }
        public string Bio { get; private set; }

        public IEnumerable<Match> Matches => _matches;
        private readonly HashSet<Match> _matches = new();

        public IEnumerable<Photo> Photos => _photos;
        private readonly HashSet<Photo> _photos = new();

        public User(int id, string phone, string email, string name, int age, Sex sex, string job="", string bio="")
        {
            Id = id;
            SetPhone(phone);
            SetEmail(email);
            SetName(name);
            Sex = sex;
            ChangeAge(age);
            ChangeBio(bio);
            ChangeJob(job);
        }

        public bool IsVisible()
        {
            return _photos.Any(x => x.Oridinal == 1);
        }
        public void ChangeAge(int age)
        {
            SetAge(age);
        }
        public void ChangeBio(string bio)
        {
            SetBio(bio);
        }
        public void ChangeJob(string job)
        {
            SetJob(job);
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
    
        #region Setters
        private void SetName(string name)
        {
            if (name.Length == 0)
            {
                throw new Exception("name must be set");
            }
            if (name.Length > 15)
            {
                throw new Exception("name must be maximum 15 characters long");
            }
            if (!NameRegex.IsMatch(name))
            {
                throw new Exception($"name has invalid characters {name}");
            }
            if (Name == name) return;
            Name = name;
        }
        private void SetPhone(string phone)
        {       
            if (phone.Length == 0)
            {
                throw new Exception("phone number must be set");
            }
            if (phone.Length > 9)
            {
                throw new Exception("phone number must be maximum 9 characters long");
            }
            if (!PhoneRegex.IsMatch(phone))
            {
                throw new Exception($"phone number must be only numbers");
            }
            if (Phone == phone) return;
            Phone = phone;
        }
        private void SetEmail(string email)
        {
            if (email.Length == 0)
            {
                throw new Exception("email must be set");
            }
            if (!EmailRegex.IsMatch(email))
            {
                throw new Exception($"invalid email address {email}");
            }
            if (Email == email) return;
            Email = email.Trim().ToLowerInvariant();
        }
        private void SetAge(int age)
        {
            if (age<18 | age >100) throw new Exception("age must be between 18 and 100");
            if (Age == age) return;
            Age = age;
        }
        private void SetJob(string job)
        {
            if (job.Length > 30) throw new Exception("job must be maximum 30 characters long");
            if (Job == job) return;
            Job = job;
        }
        private void SetBio(string bio)
        {
            if (bio.Length > 400) throw new Exception("bio must be maximum 400 characters long");
            if (Bio == bio) return;
            Bio = bio;
        }
        #endregion
    }
}