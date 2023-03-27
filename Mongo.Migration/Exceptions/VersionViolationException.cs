using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Resources.Exceptions;

namespace Mongo.Migration.Exceptions
{
    public class VersionViolationException : Exception
    {
        public VersionViolationException(
            DocumentVersion currentVersion,
            DocumentVersion documentVersion,
            DocumentVersion latestVersion)
            : base(string.Format(ErrorTexts.DuplicateVersion, currentVersion, documentVersion, latestVersion))
        {
        }
    }
}