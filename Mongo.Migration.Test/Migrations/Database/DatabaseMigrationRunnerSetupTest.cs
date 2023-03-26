using Mongo.Migration.Documents.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using NLog;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database
{
    [SetUpFixture]
    public class DatabaseMigrationRunnerSetupTest
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
                _logger.Warn(ex);
            }
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            // Do logout here
        }
    }
}