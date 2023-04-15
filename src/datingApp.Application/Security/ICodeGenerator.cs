using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;

namespace datingApp.Application.Security;

public interface ICodeGenerator
{
    public CodeDto GenerateCode(string email="", string phone="");
}