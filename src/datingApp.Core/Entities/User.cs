using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;

namespace datingApp.Core.Entities;

public class User
{
    private static readonly Regex BadPhoneRegex = new Regex(@"[^0-9]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex BadNameRegex = new Regex(@"[^a-zA-Z\s]",
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
        SetSex(sex);
        SetDateOfBirth(dateOfBirth);
        SetJob(job);
        SetBio(bio);
    }

    public bool IsVisible()
    {
        return _photos.Any(x => x.Oridinal == 1);
    }
    public int GetAge()
    {
        DateOnly currDate = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
        var age = CalculateAge(DateOfBirth, currDate);
        return age;
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
            throw new InvalidUsernameException("user name can't be empty");
        }
        if (name.Length > 15)
        {
            throw new InvalidUsernameException("user name too long");
        }
        if (BadNameRegex.IsMatch(name))
        {
            throw new InvalidUsernameException($"contains forbidden characters {name}");
        }
        if (Name == name) return;
        Name = name;
    }
    private void SetPhone(string phone)
    {       
        if (phone.Length == 0)
        {
            throw new InvalidPhoneException("phone number cannot be empty");
        }
        if (phone.Length > 9)
        {
            throw new InvalidPhoneException("phone number cannot exceed 9 characters in length");
        }
        if (BadPhoneRegex.IsMatch(phone))
        {
            throw new InvalidPhoneException("phone number must be only numbers");
        }
        if (Phone == phone) return;
        Phone = phone;
    }
    private void SetEmail(string email)
    {
        if (email.Length == 0)
        {
            throw new InvalidEmailException("email address cannot be empty");
        }
        if (email.Length > 256)
        {
            throw new InvalidEmailException("email cannot exceed 256 characters in length");
        }
        
        email = email.Trim().ToLowerInvariant();
        var emailAttrib = new EmailAddressAttribute();
        if (!emailAttrib.IsValid(email))
        {
            throw new InvalidEmailException($"invalid email address {email}");
        }
        if (Email == email) return;
        Email = email;
    }
    private void SetDateOfBirth(DateOnly dateOfBirth)
    {
        DateOnly currDate = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
        var age = CalculateAge(dateOfBirth, currDate);
        if (age < 18 | age > 100) 
        {
            throw new InvalidDateOfBirthException($"user cannot be younger than 18 or older than 100");
        }
        if (DateOfBirth == dateOfBirth) return;
        DateOfBirth = dateOfBirth;
    }
    private void SetJob(string job)
    {
        if (job.Length > 30)
        {
            throw new JobTooLongException();
        }
        if (Job == job) return;
        Job = job;
    }
    private void SetBio(string bio)
    {
        if (bio.Length > 400)
        {
            throw new BioTooLongException();
        }
        if (Bio == bio) return;
        Bio = bio;
    }
    private void SetSex(Sex sex)
    {
        if (sex == (Sex.Male & Sex.Female))
        {
            throw new InvalidUserSexException();
        }
        if (Sex == sex) return;
        Sex = sex;
    }
    #endregion
    private static int CalculateAge(DateOnly olderDate, DateOnly newerDate)
    {
        var age = newerDate.Year - olderDate.Year;
        switch (newerDate.Month - olderDate.Month)
        {
            case < 0:
                age -= 1;
                break;
            case 0:
                if (newerDate.Day < olderDate.Day)
                {
                    age -= 1;
                }
                break;
        }
        return age;
    }
}