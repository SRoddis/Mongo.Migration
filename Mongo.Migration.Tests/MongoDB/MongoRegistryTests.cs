using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Bson;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Services;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Mongo.Migration.Tests.MongoDB;

/// <summary>
/// Test that static serializers are registered (called from <see cref="TestcontainersContext"/>)
/// </summary>
[TestFixture]
internal class MongoRegistryTests : IntegrationTest
{
    [Test]
    public void DocumentVersionSerializerIsRegistered()
    {
        Assert.That(
            BsonSerializer.SerializerRegistry.GetSerializer<DocumentVersion>(),
            Is.TypeOf<DocumentVersionSerializer>());
    }

    [Test]
    public void MongoSerializerProviderIsRegistered()
    {
        Assert.That(
            BsonSerializer.SerializerRegistry.GetSerializer<TestDocumentWithOneMigration>(),
            Is.TypeOf<MigrationDocumentSerializer<TestDocumentWithOneMigration>>());
    }

    [Test]
    public void WhenInitializeTwiceThrows()
    {
        var migrationService = TestcontainersContext.Provider.GetRequiredService<IMigrationService>();
        Assert.Throws<BsonSerializationException>(migrationService.RegisterBsonStatics);
    }
}