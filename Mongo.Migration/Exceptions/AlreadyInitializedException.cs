using System;
using Mongo.Migration.Resources.Exceptions;

namespace Mongo.Migration.Exceptions
{
    public class AlreadyInitializedException : Exception
    {
        public AlreadyInitializedException()
            : base(string.Format(ErrorTexts.AlreadyInitialized))
        {
        }
    }
}