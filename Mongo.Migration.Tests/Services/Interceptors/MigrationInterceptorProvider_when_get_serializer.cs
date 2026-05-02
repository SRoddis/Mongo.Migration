using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Bson;
using Mongo.Migration.Tests.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Services.Interceptors;

[TestFixture]
internal class MigrationInterceptorProviderWhenGetSerializer : IntegrationTest
{
    [Test]
    public void When_entity_is_document_Then_provide_serializer()
    {
        // Arrange 
        var serializationProvider = TestcontainersContext.Provider.GetRequiredService<MigrationBsonSerializerProvider>();

        // Act
        var serializer = serializationProvider.GetSerializer(typeof(TestDocumentWithOneMigration));

        // Assert
        Assert.That(serializer.ValueType, Is.EqualTo(typeof(TestDocumentWithOneMigration)));
    }

    [Test]
    public void When_entity_is_not_document_Then_provide_null()
    {
        // Arrange 
        var serializationProvider = TestcontainersContext.Provider.GetRequiredService<MigrationBsonSerializerProvider>();

        // Act
        var serializer = serializationProvider.GetSerializer(typeof(TestClassNoMigration));

        // Assert
        Assert.That(serializer, Is.Null);
    }

    [Test]
    public void When_entity_is_not_document_but_manually_added_Then_provide_serializer()
    {
        // Arrange 
        var serializationProvider = TestcontainersContext.Provider.GetRequiredService<MigrationBsonSerializerProvider>();

        // Act
        var serializer = serializationProvider.GetSerializer(typeof(TestClassWithTwoMigrationMiddleVersion));

        // Assert
        Assert.That(serializer.ValueType, Is.EqualTo(typeof(TestClassWithTwoMigrationMiddleVersion)));
    }
}