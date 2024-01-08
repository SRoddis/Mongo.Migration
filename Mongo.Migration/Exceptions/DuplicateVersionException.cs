using System;

namespace Mongo.Migration.Exceptions;

internal class DuplicateVersionException : Exception
{
    public DuplicateVersionException(string typeName, string version)
        : base(string.Format(ErrorTexts.DuplicateVersion, typeName, version))
    {
    }
}