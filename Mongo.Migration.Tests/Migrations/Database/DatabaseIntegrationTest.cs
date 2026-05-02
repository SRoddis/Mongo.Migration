using Mongo.Migration.Migrations.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
internal abstract class DatabaseIntegrationTest
{
    private const string MigrationsCollectionName = "_migrations";

    private IMongoDatabase? _db;
    protected IMongoDatabase Db => _db ?? throw new InvalidOperationException("Must be setup");

    protected virtual string DatabaseName { get; set; } = "DatabaseMigration";

    protected virtual string CollectionName { get; set; } = "Test";
    
    protected async Task OnSetUpAsync()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        _db = client.GetDatabase(DatabaseName);
        await _db.CreateCollectionAsync(CollectionName);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        IMongoDatabase database = client.GetDatabase(DatabaseName);
        await database.DropCollectionAsync(CollectionName);
        await database.DropCollectionAsync(MigrationsCollectionName);
    }

    protected void InsertMigrations(IEnumerable<DatabaseMigration> migrations)
    {
        var list = migrations.Select(m => new BsonDocument { { "MigrationId", m.GetType().ToString() }, { "Version", m.Version.ToString() } });
        Db.GetCollection<BsonDocument>(MigrationsCollectionName)
            .InsertManyAsync(list).Wait();
    }

    protected List<MigrationHistory> GetMigrationHistory()
    {
        var migrationHistoryCollection = Db.GetCollection<MigrationHistory>(MigrationsCollectionName);
        return migrationHistoryCollection.Find(m => true).ToList();
    }
}