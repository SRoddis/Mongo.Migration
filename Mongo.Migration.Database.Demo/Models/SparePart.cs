using MongoDB.Bson;

namespace Mongo.Migration.Database.Demo
{
    public class SparePart
    {
        public ObjectId Id { get; set; }

        public string Title { get; set; }

        public int Count { get; set; }
    }
}