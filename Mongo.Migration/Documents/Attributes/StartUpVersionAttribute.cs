namespace Mongo.Migration.Documents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StartUpVersionAttribute : Attribute
{
    public DocumentVersion Version { get; }

    public StartUpVersionAttribute(string version)
    {
        Version = DocumentVersion.Parse(version.AsSpan());
    }
}