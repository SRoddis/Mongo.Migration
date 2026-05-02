using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations.Document;

public abstract class DocumentMigration<TClass> : IDocumentMigration
{
    protected DocumentMigration(DocumentVersion version)
    {
        Version = version;
    }

    protected DocumentMigration(ReadOnlySpan<char> span)
        : this(DocumentVersion.Parse(span)) { }

    public DocumentVersion Version { get; }

    public Type Type { get; } = typeof(TClass);

    public abstract void Up(BsonDocument document);

    public abstract void Down(BsonDocument document);
}