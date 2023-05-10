using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class MatchDto
{
    public Guid Id { get; set; }
    public PublicUserDto User { get; set; }
    public bool IsDisplayed { get; set; }
    public IEnumerable<MessageDto> Messages { get; set; }
    public DateTime CreatedAt { get; set; }
}