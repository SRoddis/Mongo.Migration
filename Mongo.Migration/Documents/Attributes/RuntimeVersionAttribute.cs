namespace Mongo.Migration.Documents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RuntimeVersionAttribute : Attribute
{
    public DocumentVersion Version { get; }

    public RuntimeVersionAttribute(string version)
    {
        Version = DocumentVersion.Parse(version.AsSpan());
    }
}