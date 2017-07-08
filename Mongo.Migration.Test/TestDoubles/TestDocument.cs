using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Test.TestDoubles
{
    [CurrentVersion("0.0.1")]
    internal class TestDocument : Document
    {
        public int Doors { get; set; }
    }

    internal class TestDocumentWithoutAttribute : Document
    {
        public int Doors { get; set; }
    }
}