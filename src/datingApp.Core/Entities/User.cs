using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using datingApp.Core.Consts;
using datingApp.Core.Exceptions;
using datingApp.Core.ValueObjects;

namespace datingApp.Core.Entities;

public class User
{
    public UserId Id { get; }
    public Phone Phone { get; private set; }
    public Email Email { get; private set; }
    public Name Name { get; private set; }
    public DateOfBirth DateOfBirth { get; private set; }
    public UserSex Sex { get; private set; }
    public string Job { get; private set; }
    public string Bio { get; private set; }
    public IEnumerable<Photo> Photos { get; private set; } = new List<Photo>();
    public UserSettings Settings { get; private set; }

    private User()
    {
        // EF
    }

    public User(UserId id, Phone phone, Email email, Name name, DateOfBirth dateOfBirth, UserSex sex,
                IEnumerable<Photo> photos, UserSettings settings, string job="", string bio="")
    {
        Id = id;
        Phone = phone;
        Email = email;
        Name = name;
        SetSex(sex);
        DateOfBirth = dateOfBirth;
        Photos = photos;
        SetSettings(settings);
        SetJob(job);
        SetBio(bio);
    }

    public int GetAge()
    {
        return DateOfBirth.GetAge();
    }

    public void ChangeDateOfBirth(DateOfBirth dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
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
    private void SetSettings(UserSettings settings)
    {
        if (settings == null) throw new UserSettingsIsNullException();
        if (Settings == settings) return;
        Settings = settings;
    }
    private void SetJob(string job)
    {
        if (job.Length > 50)
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
    private void SetSex(UserSex sex)
    {
        if (!Enum.IsDefined(typeof(UserSex), sex))
        {
            throw new InvalidUserSexException();
        }
        if (Sex == sex) return;
        Sex = sex;
    }
    #endregion
}