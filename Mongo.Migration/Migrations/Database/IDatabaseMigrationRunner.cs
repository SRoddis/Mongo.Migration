using Mongo.Migration.Documents;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database
{
    internal interface IDatabaseMigrationRunner
    {
        void Run(IMongoDatabase db, DocumentVersion databaseMigrationVersion);
    }
}