using Mongo.Migration.Documents;

namespace Mongo.Migration.Exceptions;

public class VersionViolationException : Exception
{
    public VersionViolationException(
        DocumentVersion currentVersion,
        DocumentVersion documentVersion,
        DocumentVersion latestVersion)
        : base($"CurrentVersion: '{currentVersion}', DocumentVersion: '{documentVersion}', LatestMigrationVersion: '{latestVersion}'")
    {
    }
}