using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Attributes;
using MongoDB.Bson;

namespace Mongo.Migration.Demo.Model
{
    [RuntimeVersion("0.1.1")]
    [StartUpVersion("0.0.1")]
    [CollectionLocation("Car", "TestCars")]
    public class Car : IDocument
    {
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public int Doors { get; set; }
        
        public string UnnecessaryField { get; set; }
        
        public DocumentVersion Version { get; set; }
        
    }
}