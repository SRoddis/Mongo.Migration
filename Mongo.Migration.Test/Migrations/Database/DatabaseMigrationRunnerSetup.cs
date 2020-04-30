using Mongo.Migration.Documents.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database
{
    [SetUpFixture]
    public class DatabaseMigrationRunnerSetup
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            try
            {
                var documentSerializaer = new DocumentVersionSerializer();
                BsonSerializer.RegisterSerializer(documentSerializaer.ValueType, documentSerializaer);
            }
            catch (BsonSerializationException ex)
            {
            }
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            // Do logout here
        }
    }
}