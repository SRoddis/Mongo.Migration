using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Test.TestDoubles
{
    [RuntimeVersion("0.0.2")]
    [CollectionLocation("Test", "PerformanceTest")]
    internal class TestDocumentWithTwoMigrationHighestVersion : Document
    {
        public ObjectId Id { get; set; }

        public int Door { get; set; }
    }
}