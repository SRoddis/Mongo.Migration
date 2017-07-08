using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Test.TestDoubles
{
    [CurrentVersion("0.0.2")]
    internal class TestDocumentWithTwoMigrationHighestVersion : Document
    {
        public int Door { get; set; }
    }
}