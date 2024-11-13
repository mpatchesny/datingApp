using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal sealed class PhotoReadModel
{
    public Guid Id { get; set; }
    public PublicUserReadModel User { get; set; }
    public string Url { get; set; }
    public int Oridinal { get; set; }
}