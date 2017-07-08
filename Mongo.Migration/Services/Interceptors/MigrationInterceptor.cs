using Mongo.Migration.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Services.Interceptors
{
    public class MigrationInterceptor<TClass> : BsonClassMapSerializer<TClass> where TClass : class, IDocument
    {
        //private readonly IMigrationRunner _migrationRunner;

        public MigrationInterceptor()
            : base(BsonClassMap.LookupClassMap(typeof (TClass)))
        {
            //_migrationRunner = FlyingMigratory.CreateMigrationRunner();
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TClass value)
        {
            //_migrationRunner.CheckVersion(value);

            base.Serialize(context, args, value);
        }

        public override TClass Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // TODO: Performance? LatestVersion, dont do anything
            BsonDocument document = BsonDocumentSerializer.Instance.Deserialize(context);

            //_migrationRunner.Run(typeof(TClass), document);

            BsonDeserializationContext migratedContext =
                BsonDeserializationContext.CreateRoot(new BsonDocumentReader(document));

            return base.Deserialize(migratedContext, args);
        }
    }
}