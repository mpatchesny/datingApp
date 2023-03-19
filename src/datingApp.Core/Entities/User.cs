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
        public DateOnly DateOfBirth { get; private set; }
        public Sex Sex { get; private set; }
        public string Job { get; private set; }
        public string Bio { get; private set; }

        public IEnumerable<Match> Matches => _matches;
        private readonly HashSet<Match> _matches = new();

        public IEnumerable<Photo> Photos => _photos;
        private readonly HashSet<Photo> _photos = new();

        public User(int id, string phone, string email, string name, DateOnly dateOfBirth, Sex sex, string job="", string bio="")
        {
            Id = id;
            SetPhone(phone);
            SetEmail(email);
            SetName(name);
            Sex = sex;
            SetDateOfBirth(dateOfBirth);
            SetBio(bio);
            SetJob(job);
        }

        public bool IsVisible()
        {
            return _photos.Any(x => x.Oridinal == 1);
        }
        public int Age()
        {
            return 0;
        }

        public void ChangeDateOfBirth(DateOnly dateOfBirth)
        {
            SetDateOfBirth(dateOfBirth);
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
                throw new Exception("name cannot exceed 15 characters in length");
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
                throw new Exception("phone number cannot be empty");
            }
            if (phone.Length > 9)
            {
                throw new Exception("phone number cannot exceed 9 characters in length");
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
                throw new Exception("email address cannot be empty");
            }
            if (!EmailRegex.IsMatch(email))
            {
                throw new Exception($"invalid email address {email}");
            }
            if (Email == email) return;
            Email = email.Trim().ToLowerInvariant();
        }
        private void SetDateOfBirth(DateOnly dateOfBirth)
        {
            DateOnly currDate = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            var age = currDate.Year - dateOfBirth.Year;
            switch (currDate.Month - dateOfBirth.Month)
            {
                case < 0:
                    age -= 1;
                    break;
                case 0:
                    if ((currDate.Day - dateOfBirth.Day) < 0)
                    {
                        age -= 1;
                    }
                    break;
            }
            if (age < 18 | age > 100) throw new Exception("invalid date of birth; user age must be between 18 and 100");
            if (DateOfBirth == dateOfBirth) return;
            DateOfBirth = dateOfBirth;
        }
        private void SetJob(string job)
        {
            if (job.Length > 30) throw new Exception("job cannot exceed 30 characters in length");
            if (Job == job) return;
            Job = job;
        }
        private void SetBio(string bio)
        {
            if (bio.Length > 400) throw new Exception("bio cannot exceed 400 characters in length");
            if (Bio == bio) return;
            Bio = bio;
        }
        #endregion
    }
}