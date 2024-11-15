using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal sealed class UserReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Sex { get; set; }
    public string Job { get; set; }
    public string Bio { get; set; }
    public int Age { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int Distance { get; set; }
    public UserSettingsReadModel Settings { get; set; }
    public IEnumerable<PhotoReadModel> Photos { get; set; }
    public IEnumerable<MatchReadModel> Matches 
    { 
        get 
        { 
            return (Matches1 ?? new List<MatchReadModel>()).Union(Matches2 ?? new List<MatchReadModel>()) ; 
        } 
    }
    internal IEnumerable<MatchReadModel> Matches1 { get; set; }
    internal IEnumerable<MatchReadModel> Matches2 { get; set; }
}

internal sealed class UserSettingsReadModel
{
    internal Guid UserId { get; set; }
    public UserReadModel User { get; set; }
    public int PreferredSex { get; set; }
    public int PreferredAgeFrom { get; set; }
    public int PreferredAgeTo { get; set; }
    public int PreferredMaxDistance { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}