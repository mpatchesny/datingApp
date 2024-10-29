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
    public Job Job { get; private set; }
    public Bio Bio { get; private set; }
    public ICollection<Photo> Photos { get; private set; } = new List<Photo>();
    public UserSettings Settings { get; private set; }

    private User()
    {
        // EF
    }

    public User(UserId id, Phone phone, Email email, Name name, DateOfBirth dateOfBirth, UserSex sex,
                ICollection<Photo> photos, UserSettings settings, Job job=null, Bio bio=null)
    {
        if (!Enum.IsDefined(typeof(UserSex), sex)) throw new InvalidUserSexException();
        if (settings == null) throw new UserSettingsIsNullException();

        Id = id;
        Phone = phone;
        Email = email;
        Name = name;
        Sex = sex;
        DateOfBirth = dateOfBirth;
        Photos = photos;
        Settings = settings;
        Job = job ?? new Job("");
        Bio = bio ?? new Bio("");
    }

    public int GetAge()
    {
        return DateOfBirth.GetAge();
    }

    public void ChangeDateOfBirth(DateOfBirth dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void ChangeBio(Bio bio)
    {
        Bio = bio;
    }

    public void ChangeJob(Job job)
    {
        Job = job;
    }

    public void AddPhoto(Photo photo)
    {
        if (Photos.Count >= 6)
        {
            throw new UserPhotoLimitException();
        }
        Photos.Add(photo);
    }

    public void RemovePhoto(PhotoId photoId)
    {
        var photo = Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo != null) Photos.Remove(photo);
    }

    public void ChangeOridinal(PhotoId photoId, Oridinal newOridinal)
    {
        var photo = Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null) return;

        photo.ChangeOridinal(newOridinal);

        foreach (var p in Photos.Where(p => p.Id != photoId))
        {
            if (p.Oridinal >= photo.Oridinal)
            {
                p.ChangeOridinal(p.Oridinal.Value+1);
            }
            else if (p.Oridinal < photo.Oridinal)
            {
                p.ChangeOridinal(p.Oridinal.Value-1);
            }
        }
    }
}