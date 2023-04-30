using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class PrivateUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Age { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int Sex { get; set; }
    public string Job { get; set; }
    public string Bio { get; set; }
    public IEnumerable<PhotoDto> Photos { get; set; }
    public UserSettingsDto Settings { get; set; }
}