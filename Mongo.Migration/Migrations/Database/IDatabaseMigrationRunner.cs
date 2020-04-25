using Mongo.Migration.Documents;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations
{
    internal interface IAdvancedMigrationRunner
    {
        void Run(IMongoDatabase db, DocumentVersion runnigVersion);
    }
}