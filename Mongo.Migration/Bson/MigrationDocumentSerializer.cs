using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson.Serialization;
using Mongo.Migration.Services;

namespace Mongo.Migration.Bson;
internal sealed class MigrationDocumentSerializer<TDocument> : BaseMigrationSerializer<TDocument>
    where TDocument : IDocument
{
    public MigrationDocumentSerializer(IDocumentMigrationRunner migrationRunner, IDocumentVersionService documentVersionService)
    : base(migrationRunner, documentVersionService) { }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDocument value)
    {
        value.Version = RuntimeVersion;
        InnerSerializer.Serialize(context, args, value);
    }
}
