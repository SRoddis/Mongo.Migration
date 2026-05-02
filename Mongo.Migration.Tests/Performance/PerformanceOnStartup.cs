using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Performance;

[TestFixture]
public class PerformanceTestOnStartup
{
    private const int DocumentCount = 5000;

    private const string DatabaseName = "PerformanceTest";

    private const string CollectionName = "Test";

    private const int ToleranceMs = 2800;

    [Test]
    public async Task When_migrating_number_of_documents()
    {
        // Arrange
        // Worm up MongoCache
        ClearCollection();
        await AddDocumentsToCacheAsync();
        ClearCollection();

        // Act
        // Measure time of MongoDb processing without Mongo.Migration
        await InsertDocumentsAsync(DocumentCount);
        var sw = new Stopwatch();
        sw.Start();
        var _ = await QueryAllAsync(false);
        sw.Stop();

        ClearCollection();

        // Measure time of MongoDb processing with Mongo.Migration
        IMongoClient client = TestcontainersContext.MongoClient;
        await InsertDocumentsAsync(DocumentCount);
        var swWithMigration = new Stopwatch();
        swWithMigration.Start();
        
        IStartUpDocumentMigrationRunner documentMigrationRunner =
            TestcontainersContext.Provider.GetRequiredService<IStartUpDocumentMigrationRunner>();
        await documentMigrationRunner.RunAllAsync(client.GetDatabase(DatabaseName), CancellationToken.None);
        swWithMigration.Stop();

        ClearCollection();

        var result = swWithMigration.ElapsedMilliseconds - sw.ElapsedMilliseconds;

        await TestContext.Out.WriteLineAsync($"MongoDB: {sw.ElapsedMilliseconds}ms, Mongo.Migration: {swWithMigration.ElapsedMilliseconds}ms, Diff: {result}ms (Tolerance: {ToleranceMs}ms), Documents: {DocumentCount}, Migrations per Document: 2");
        
        // Assert
        Assert.That(result, Is.LessThan(ToleranceMs));
    }

    private static Task InsertDocumentsAsync(int documentCount)
    {
        var documents = Enumerable
            .Range(0, documentCount)
            .Select(i => new BsonDocument
            {
                { "_id", new BsonObjectId(ObjectId.GenerateNewId())},
                { "Dors", new BsonInt32(i) },
                { "Version", BsonString.Create("0.0.0") }
            });

        return TestcontainersContext.MongoClient
            .GetDatabase(DatabaseName)
            .GetCollection<BsonDocument>(CollectionName)
            .InsertManyAsync(documents);
    }

    private static async Task<List<object>> QueryAllAsync(bool withVersion)
    {
        IMongoClient client = TestcontainersContext.MongoClient;

        if (withVersion)
        {
            var versionedCollection = client.GetDatabase(DatabaseName)
                .GetCollection<TestDocumentWithTwoMigrationHighestVersion>(CollectionName);
            var versionedResult = await (await versionedCollection.FindAsync(_ => true)).ToListAsync();
            return [.. versionedResult.Cast<object>()];
        }

        var collection = client.GetDatabase(DatabaseName)
            .GetCollection<TestClassNoMigration>(CollectionName);
        var result = await (await collection.FindAsync(_ => true)).ToListAsync();
        return [.. result.Cast<object>()];
    }

    private static async Task AddDocumentsToCacheAsync()
    {
        await InsertDocumentsAsync(DocumentCount);
        await QueryAllAsync(false);
    }

    private static void ClearCollection()
    {
        TestcontainersContext.MongoClient
            .GetDatabase(DatabaseName)
            .DropCollection(CollectionName);
    }
}