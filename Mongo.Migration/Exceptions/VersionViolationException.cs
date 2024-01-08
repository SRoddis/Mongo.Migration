using System;

using Mongo.Migration.Documents;

namespace Mongo.Migration.Exceptions;

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