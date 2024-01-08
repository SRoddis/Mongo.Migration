using System;

namespace Mongo.Migration.Exceptions;

public class VersionStringToLongException : Exception
{
    public VersionStringToLongException(string version)
        :
        base(string.Format(ErrorTexts.VersionStringToLong, version))
    {
    }
}