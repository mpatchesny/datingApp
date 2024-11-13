using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Infrastructure.DAL.Models;

internal sealed class PublicUserReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Sex { get; set; }
    public string Job { get; set; }
    public string Bio { get; set; }
    public int Age { get; set; }
    public int Distance { get; set; }
    public IEnumerable<PhotoReadModel> Photos { get; set; }
}