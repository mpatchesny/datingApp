using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

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
    public IEnumerable<Photo> Photos { get; private set; }
    public UserSettings Settings { get; private set; }

    private User()
    {
        // EF
    }
    public User(int id, string phone, string email, string name, DateOnly dateOfBirth, Sex sex,
                IEnumerable<Photo> photos, UserSettings settings, string job="", string bio="")
    {
        Id = id;
        SetPhone(phone);
        SetEmail(email);
        SetName(name);
        SetSex(sex);
        SetDateOfBirth(dateOfBirth);
        if (photos == null) Photos = new List<Photo>();
        Photos = photos;
        SetSettings(settings);
        SetJob(job);
        SetBio(bio);
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
            throw new InvalidPhoneException("phone number too long");
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
            throw new InvalidEmailException("email too long");
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
            throw new InvalidDateOfBirthException($"user cannot be younger than 18 or older than 100 years");
        }
        if (DateOfBirth == dateOfBirth) return;
        DateOfBirth = dateOfBirth;
    }
    private void SetSettings(UserSettings settings)
    {
        if (settings == null) throw new UserSettingsIsNullException();
        if (Settings == settings) return;
        Settings = settings;
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