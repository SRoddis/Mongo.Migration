using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Migration.OnDeserialization
{
    internal class MigrationOnDeserialization : AbstractMigrationStrategy
    {
        public MigrationOnDeserialization(DocumentVersionSerializer serializer, MigrationInterceptorProvider provider) 
            : base(serializer, provider)
        {}

        protected override void OnMigrate()
        {}
    }
}