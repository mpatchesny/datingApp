using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class PhotoDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Path { get; set; }
    public int Oridinal { get; set; }
}