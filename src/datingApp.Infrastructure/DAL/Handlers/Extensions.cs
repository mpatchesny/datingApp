using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Core.Entities;

namespace datingApp.Infrastructure.DAL.Handlers;

public static class Extensions
{
    public static PublicUserDto AsDto(this User entity)
    {
        return new()
        {
            Id = entity.Id,
            Age = entity.GetAge(),
            Bio = entity.Bio,
            Distance = 0, // FIXME
            Job = entity.Job,
            Name = entity.Name,
            Sex = (int) entity.Sex,
            Photos = entity.Photos.Select(photo => photo.AsDto()).ToList()
        };
    }

    public static PhotoDto AsDto(this Photo entity)
    {
        return new()
        {
            Id = entity.Id,
            Oridinal = entity.Oridinal,
            Path = entity.Path,
            UserId = entity.UserId
        };
    }
}