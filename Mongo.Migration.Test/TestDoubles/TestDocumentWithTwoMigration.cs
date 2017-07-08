using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Test.TestDoubles
{
    [CurrentVersion("0.0.0")]
    internal class TestDocumentWithTwoMigration : Document
    {
        public int Dors { get; set; }
    }
}