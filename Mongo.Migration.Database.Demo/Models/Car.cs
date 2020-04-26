using MongoDB.Bson;

namespace Mongo.Migration.Database.Demo
{
    public class Car
    {
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public int Doors { get; set; }
    }
}