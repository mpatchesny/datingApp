using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class PhotoDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Path { get; set; }
    public int Oridinal { get; set; }
}