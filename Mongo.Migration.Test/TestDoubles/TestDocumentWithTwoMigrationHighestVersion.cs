using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Test.TestDoubles
{
    [CurrentVersion("0.0.2")]
    [DatabaseLocation("PerformanceTest","Test")]
    internal class TestDocumentWithTwoMigrationHighestVersion : Document
    {
        public ObjectId Id { get; set; }

        public int Door { get; set; }
    }
}