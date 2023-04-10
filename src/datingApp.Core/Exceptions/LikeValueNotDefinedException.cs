using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class LikeValueNotDefinedException : CustomException
{
    public LikeValueNotDefinedException(int like) : base($"Value {like} is not defined.")
    {
    }
}