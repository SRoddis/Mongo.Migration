using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    public abstract class Migration<TClass> : IMigration where TClass : class, IDocument
    {
        protected Migration(string version)
        {
            Version = version;
        }

        public DocumentVersion Version { get; }

        public Type Type => typeof(TClass);

        public abstract void Up(BsonDocument document);

        public abstract void Down(BsonDocument document);
    }
}