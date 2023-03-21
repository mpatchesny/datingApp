using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class PublicUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public int Sex { get; set; }
    public int Distance { get; set; }
    public string Job { get; set; }
    public string Bio { get; set; }
    public IEnumerable<Object> Photos { get; set; }
}