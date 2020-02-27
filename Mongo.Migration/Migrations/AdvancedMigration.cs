using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    public abstract class AdvancedMigration : IAdvancedMigration
    {
        protected AdvancedMigration(string version, string collection)
        {
            Version = version;
            CollectionName = collection;
        }

        public string Version { get; }

        public string CollectionName { get; }

        public Type Type => typeof(AdvancedMigration);

        public virtual void Up(IMongoDatabase db) {
            var bulk = new List<WriteModel<BsonDocument>>();
            var collection = db.GetCollection<BsonDocument>(CollectionName);

            collection.FindSync(_ => true).ToList().ForEach(document =>
            {
               Up(document);

                var update = new ReplaceOneModel<BsonDocument>(
                    new BsonDocument {{"_id", document["_id"]}},
                    document
                );

                bulk.Add(update);
            });

            if (bulk.Count > 0)
            {
                collection.BulkWrite(bulk);
            }
        }

        public virtual void Down(IMongoDatabase db) {
            var bulk = new List<WriteModel<BsonDocument>>();
            var collection = db.GetCollection<BsonDocument>(CollectionName);

            collection.FindSync(_ => true).ToList().ForEach(document =>
            {
               Down(document);

                var update = new ReplaceOneModel<BsonDocument>(
                    new BsonDocument {{"_id", document["_id"]}},
                    document
                );

                bulk.Add(update);
            });

            if (bulk.Count > 0)
            {
                collection.BulkWrite(bulk);
            }
        }

        public abstract void Up(BsonDocument document);
        
        public abstract void Down(BsonDocument document);
    }
}