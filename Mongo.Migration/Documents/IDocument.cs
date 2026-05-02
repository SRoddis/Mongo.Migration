namespace Mongo.Migration.Documents;

public interface IDocument
{
    DocumentVersion Version { get; set; }
}