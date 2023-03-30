using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Demo.Model.Pkg
{
    [RuntimeVersion("0.1.1")]
    [StartUpVersion("0.0.1")]
    [CollectionLocation("Car", "Local-test")]
    public class Car : IDocument
    {
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public int Doors { get; set; }

        public DocumentVersion Version { get; set; }
    }
}