namespace Mongo.Migration.Documents.Locators
{
    internal interface IVersionLocator
    {
        DocumentVersion? GetCurrentVersion<TDocument>() where TDocument : class, IDocument;
    }
}