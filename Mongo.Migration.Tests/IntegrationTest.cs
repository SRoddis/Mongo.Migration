using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests;

[TestFixture]
public abstract class IntegrationTest
{
    private const string DatabaseName = "IntegrationTest";
    private const string CollectionName = "Test";

    [SetUp]
    protected void SetUp()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(CollectionName);
    }

    [TearDown]
    protected async Task TearDownAsync()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        await client.GetDatabase(DatabaseName)
            .DropCollectionAsync(CollectionName);
    }
}