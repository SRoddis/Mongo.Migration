using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Test.TestDoubles;

[RuntimeVersion("0.0.1")]
internal class TestDocumentWithTwoMigrationMiddleVersion : Document
{
    public int Door { get; set; }
}