using System;
using Mongo.Migration.Documents;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database
{
    public abstract class DatabaseMigration : IDatabaseMigration
    {
        protected DatabaseMigration(string version)
        {
            Version = version;
        }

        public DocumentVersion Version { get; }

        public Type Type => typeof(DatabaseMigration);

        public abstract void Up(IMongoDatabase db);

        public abstract void Down(IMongoDatabase db);
    }
}