namespace Mongo.Migration.Models
{
    public interface IDocument
    {
        DocumentVersion Version { get; set; }
    }
}