using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Exceptions;

public class NullPhotoFileException : CustomException
{
    public NullPhotoFileException() : base("Photo file content must be provided.")
    {
    }
}