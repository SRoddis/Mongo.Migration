using System;

namespace Mongo.Migration.Exceptions;

public class AlreadyInitializedException : Exception
{
    public AlreadyInitializedException()
        : base(string.Format(ErrorTexts.AlreadyInitialized))
    {
    }
}