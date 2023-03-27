using System;
using Mongo.Migration.Resources.Exceptions;

namespace Mongo.Migration.Exceptions
{
    public class InvalidVersionValueException : Exception
    {
        public InvalidVersionValueException(string value) : base(string.Format(ErrorTexts.InvalidVersionValue, value))
        {
        }
    }
}