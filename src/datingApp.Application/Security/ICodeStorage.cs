using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;

namespace datingApp.Application.Security;

public interface ICodeStorage
{
    public void Set(AccessCodeDto code);
    public AccessCodeDto Get(string emailOrPhone);
}