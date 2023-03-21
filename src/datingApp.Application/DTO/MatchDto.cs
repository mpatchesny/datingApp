using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class MatchDto
{
    public int Id { get; set; }
    public int UserId1 { get; set; }
    public int UserId2 { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<MessageDto> Messages { get; set; }
}