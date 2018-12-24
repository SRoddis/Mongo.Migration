using Mongo.Migration.Documents.VersionProviders;

namespace Mongo.Migration.Documents
{
    public class DocumentVersionProvider : IDocumentVersionProvider<IDocument>
    {
        public DocumentVersion GetVersion(IDocument document) => document.Version;

        public void SetVersion(IDocument document, DocumentVersion version) => document.Version = version;
    }
}