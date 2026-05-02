namespace Mongo.Migration.Exceptions;

public class DuplicateVersionException : Exception
{
    public DuplicateVersionException(string typeName, string version)
        : base($"Migration '{typeName}' contains duplicate version: {version}")
    {
    }
}