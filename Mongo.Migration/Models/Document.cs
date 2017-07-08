namespace Mongo.Migration.Models
{
    public class Document : IDocument
    {
        public DocumentVersion Version { get; set; }
    }
}