namespace Mongo.Migration.Documents;

public class Document : IDocument
{
    public DocumentVersion Version { get; set; }
}