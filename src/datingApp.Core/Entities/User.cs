using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
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
    public ICollection<Photo> Photos { get; private set; }
    public UserSettings Settings { get; private set; }
    private const int PhotoCountLimit = 6;

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
        Photos = photos ?? new List<Photo>();
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
        if (Photos.Count >= PhotoCountLimit)
        {
            throw new UserPhotoLimitException();
        }

        if (Photos.Any(p => p.Id == photo.Id)) return;

        photo.ChangeOridinal(Photos.Count);
        Photos.Add(photo);
    }

    public void RemovePhoto(PhotoId photoId)
    {
        var photo = Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo != null) 
        {
            ChangeOridinal(photoId, 999);
            Photos.Remove(photo);
        }
    }

    public void ChangeOridinal(PhotoId photoId, Oridinal newOridinal)
    {
        newOridinal = Math.Min(newOridinal, Math.Max(0, Photos.Count-1));

        var photoToChange = Photos.FirstOrDefault(p => p.Id == photoId
            && p.Oridinal != newOridinal);
        if (photoToChange == null) return;

        var shiftUp = newOridinal > photoToChange.Oridinal;
        var lowerBound = shiftUp ? photoToChange.Oridinal : newOridinal;
        var upperBound = shiftUp ? newOridinal : photoToChange.Oridinal;

        foreach (var photo in Photos)
        {
            if (photo.Id != photoId && photo.Oridinal >= lowerBound 
                && photo.Oridinal <= upperBound)
            {
                var shift = shiftUp ? -1 : 1;
                photo.ChangeOridinal(photo.Oridinal + shift);
            }
        }

        photoToChange.ChangeOridinal(newOridinal);
    }
}