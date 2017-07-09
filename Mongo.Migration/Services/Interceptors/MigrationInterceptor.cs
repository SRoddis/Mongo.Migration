using System;
using System.Diagnostics;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Services.Interceptors
{
    internal class MigrationInterceptor<TClass> : BsonClassMapSerializer<TClass> where TClass : class, IDocument
    {
        private readonly IMigrationRunner _migrationRunner;

        public MigrationInterceptor(IMigrationRunner migrationRunner)
            : base(BsonClassMap.LookupClassMap(typeof(TClass)))
        {
            _migrationRunner = migrationRunner;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TClass value)
        {
            _migrationRunner.CheckVersion(value);

            base.Serialize(context, args, value);
        }

        public override TClass Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // TODO: Performance? LatestVersion, dont do anything
            var document = BsonDocumentSerializer.Instance.Deserialize(context);
            
            _migrationRunner.Run(typeof(TClass), document);
            
            var migratedContext =
                BsonDeserializationContext.CreateRoot(new BsonDocumentReader(document));

            return base.Deserialize(migratedContext, args);
        }
    }
}