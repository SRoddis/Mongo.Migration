using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Tests.TestDoubles.Database;

public class TestDatabaseMigration001 : DatabaseMigration
{
    public TestDatabaseMigration001() : base("0.0.1") { }

    public override Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;

    public override Task DownAsync(IMongoDatabase db, CancellationToken cancellationToken) => Task.CompletedTask;
}