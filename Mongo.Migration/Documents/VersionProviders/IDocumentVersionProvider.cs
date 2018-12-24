using System;

namespace Mongo.Migration.Documents.VersionProviders
{
    public interface IDocumentVersionProvider<TBaseDocument>
    {
        DocumentVersion GetVersion(TBaseDocument document);

        void SetVersion(TBaseDocument document, DocumentVersion version);
    }
}
