using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Tests.TestDoubles.Database;

public class TestDatabaseMigration003 : DatabaseMigration
{
    public TestDatabaseMigration003() : base("0.0.3") { }

    public override Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;

    public override Task DownAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;
}