
using System.Diagnostics;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Performance;

public class PerformanceRuntimeMigration
{
    private const string DatabaseName = "PerformanceTest";

    private const string CollectionName = "TestRuntime";

    [TearDown]
    public Task TearDownAsync()
    {
        return GetCollection<BsonDocument>()
            .DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
    }

    private static IMongoCollection<T> GetCollection<T>()
    {
        return TestcontainersContext.MongoClient
            .GetDatabase(DatabaseName)
            .GetCollection<T>(CollectionName);
    }

    private static Task InsertDocumentsAsync(int documentCount, Func<int, BsonDocument> documentFactory)
    {
        var documents = Enumerable
            .Range(0, documentCount)
            .Select(documentFactory);

        return GetCollection<BsonDocument>()
            .InsertManyAsync(documents);
    }

    [TestCase(1_000, 100L)]
    [TestCase(10_000, 200L)]
    [TestCase(50_000, 500L)]
    public async Task MeasureVanillaRead(int documentCount, long msDurationThreshold)
    {
        await InsertDocumentsAsync(documentCount, i => new BsonDocument
        {
            { "_id", new BsonObjectId(ObjectId.GenerateNewId())},
            { "Dors", new BsonInt32(i) },
            { "Version", BsonString.Create("0.0.0") }
        });

        IMongoCollection<TestClassNoMigration> collection = GetCollection<TestClassNoMigration>();
        Stopwatch sw = Stopwatch.StartNew();
        List<TestClassNoMigration> results = await (await collection.FindAsync(FilterDefinition<TestClassNoMigration>.Empty)).ToListAsync();
        sw.Stop();

        await TestContext.Out.WriteLineAsync($"Elapsed {sw.ElapsedMilliseconds} ms to read {results.Count} documents");
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(msDurationThreshold));
    }

    [TestCase(1_000, 200L)]
    [TestCase(10_000, 400L)]
    [TestCase(50_000, 1000L)]
    public async Task MeasureRead2Migrations(int documentCount, long msDurationThreshold)
    {
        await InsertDocumentsAsync(documentCount, i => new BsonDocument
        {
            { "_id", new BsonObjectId(ObjectId.GenerateNewId())},
            { "Dors", new BsonInt32(i) },
            { "Version", BsonString.Create("0.0.0") }
        });

        IMongoCollection<TestDocumentWithTwoMigrationHighestVersion> collection = GetCollection<TestDocumentWithTwoMigrationHighestVersion>();
        TestDocumentWithTwoMigrationHighestVersion? _ = collection.AsQueryable().FirstOrDefault();
        
        Stopwatch sw = Stopwatch.StartNew();
        List<TestDocumentWithTwoMigrationHighestVersion> results = await (await collection.FindAsync(FilterDefinition<TestDocumentWithTwoMigrationHighestVersion>.Empty)).ToListAsync();
        sw.Stop();

        await TestContext.Out.WriteLineAsync($"Elapsed {sw.ElapsedMilliseconds} ms to read {results.Count} documents");
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(msDurationThreshold));
    }

    [TestCase(1_000, 200L)]
    [TestCase(10_000, 400L)]
    [TestCase(50_000, 1000L)]
    public async Task MeasureReadNoMigration(int documentCount, long msDurationThreshold)
    {
        await InsertDocumentsAsync(documentCount, i => new BsonDocument
        {
            { "_id", new BsonObjectId(ObjectId.GenerateNewId())},
            { "Door", new BsonInt32(i) },
            { "Version", BsonString.Create("0.0.2") }
        });

        IMongoCollection<TestDocumentWithTwoMigrationHighestVersion> collection = GetCollection<TestDocumentWithTwoMigrationHighestVersion>();
        TestDocumentWithTwoMigrationHighestVersion? _ = collection.AsQueryable().FirstOrDefault();

        Stopwatch sw = Stopwatch.StartNew();
        List<TestDocumentWithTwoMigrationHighestVersion> results = await (await collection.FindAsync(FilterDefinition<TestDocumentWithTwoMigrationHighestVersion>.Empty)).ToListAsync();
        sw.Stop();

        await TestContext.Out.WriteLineAsync($"Elapsed {sw.ElapsedMilliseconds} ms to read {results.Count} documents");
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(msDurationThreshold));
    }
}