using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Driver.Linq;

namespace Mongo.Migration.Tests.MongoDB;

[TestFixture]
public class VariousCrudOperations
{
    private const string DatabaseName = "CrudTest";
    private const string CollectionName = "TestDocumentWithOneMigration";
    private IMongoCollection<TestDocumentWithOneMigration>? _collection;

    private IMongoCollection<TestDocumentWithOneMigration> Collection => _collection ?? throw new InvalidOperationException();

    [SetUp]
    public void SetUp()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        _collection = client.GetDatabase(DatabaseName)
            .GetCollection<TestDocumentWithOneMigration>(CollectionName);
    }

    [Test]
    public async Task TestCreate()
    {
        var id = ObjectId.GenerateNewId();
        var document = new TestDocumentWithOneMigration
        {
            Id = id,
            Doors = 1
        };

        await Collection.InsertOneAsync(document);

        var documentCreated = Collection.AsQueryable()
            .Where(d => d.Id == id)
            .Single();

        Assert.That(documentCreated.Id, Is.EqualTo(id));
    }

    [Test]
    public async Task TestRead()
    {
        var id = ObjectId.GenerateNewId();
        var document = new TestDocumentWithOneMigration
        {
            Id = id,
            Doors = 99
        };

        await Collection.InsertOneAsync(document);

        var documentById = Collection.AsQueryable()
            .Where(d => d.Id == id)
            .Single();

        var documentByExpression = Collection.AsQueryable()
            .Where(d => d.Doors >= 98)
            .Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(documentById.Id, Is.EqualTo(id));
            Assert.That(documentByExpression.Id, Is.EqualTo(id));
        }
    }

    [Test]
    public async Task TestUpdate()
    {
        var id = ObjectId.GenerateNewId();
        var document = new TestDocumentWithOneMigration
        {
            Id = id,
            Doors = 7
        };

        await Collection.InsertOneAsync(document);

        var documentCreated = Collection.AsQueryable()
            .Where(d => d.Id == id)
            .Single();

        Assert.That(documentCreated.Id, Is.EqualTo(id));

        var updateResult = await Collection.UpdateOneAsync(
            Builders<TestDocumentWithOneMigration>.Filter.Where(d => d.Id == id && d.Doors == 7),
            Builders<TestDocumentWithOneMigration>.Update
                .Set(d => d.Doors, 8));

        Assert.That(updateResult.MatchedCount, Is.EqualTo(1));

        var documentUpdated = Collection.AsQueryable()
            .Where(d => d.Id == id)
            .Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(documentUpdated.Id, Is.EqualTo(id));
            Assert.That(documentUpdated.Doors, Is.EqualTo(8));
        }
    }

    [Test]
    public async Task TestDelete()
    {
        var id = ObjectId.GenerateNewId();
        var document = new TestDocumentWithOneMigration
        {
            Id = id,
            Doors = 9
        };

        await Collection.InsertOneAsync(document);

        var documentCreated = Collection.AsQueryable()
            .Where(d => d.Id == id)
            .Single();

        Assert.That(documentCreated.Id, Is.EqualTo(id));

        var deleteResult = await Collection.DeleteOneAsync(d => d.Id == id);

        Assert.That(deleteResult.DeletedCount, Is.EqualTo(1));

        var searchResult = await Collection.AsQueryable()
            .Where(d => d.Id == id)
            .ToListAsync();

        Assert.That(searchResult, Is.Empty);
    }
}
