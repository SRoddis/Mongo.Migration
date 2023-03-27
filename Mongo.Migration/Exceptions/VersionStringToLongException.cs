using System;
using Mongo.Migration.Resources.Exceptions;

namespace Mongo.Migration.Exceptions
{
    public class VersionStringToLongException : Exception
    {
        public VersionStringToLongException(string version)
            :
            base(string.Format(ErrorTexts.VersionStringToLong, version))
        {
        }
    }
}