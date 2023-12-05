using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Application.DTO;

public class FileDto
{
    public String Id { get; set; }
    public String Extension { get; set; }
    public byte[] Binary { get; set; }
}