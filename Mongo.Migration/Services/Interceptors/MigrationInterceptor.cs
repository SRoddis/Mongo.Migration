using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptor<TDocument> : BsonClassMapSerializer<TDocument>
        where TDocument : class, IDocument
    {
        private readonly IDocumentVersionService _documentVersionService;
        private readonly IDocumentMigrationRunner _migrationRunner;

        public MigrationInterceptor(IDocumentMigrationRunner migrationRunner, IDocumentVersionService documentVersionService)
            : base(BsonClassMap.LookupClassMap(typeof(TDocument)))
        {
            _migrationRunner = migrationRunner;
            _documentVersionService = documentVersionService;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDocument value)
        {
            _documentVersionService.DetermineVersion(value);

            base.Serialize(context, args, value);
        }

        public override TDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var document = BsonDocumentSerializer.Instance.Deserialize(context);

            _migrationRunner.Run(typeof(TDocument), document);

            var migratedContext = BsonDeserializationContext.CreateRoot(new BsonDocumentReader(document));

            return base.Deserialize(migratedContext, args);
        }
    }
}