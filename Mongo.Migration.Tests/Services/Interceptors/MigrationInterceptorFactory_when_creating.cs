using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Bson;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Services.Interceptors;

[TestFixture]
internal class MigrationInterceptorFactoryWhenCreating : IntegrationTest
{
    [Test]
    public void If_type_is_assignable_to_document_Then_interceptor_is_created()
    {
        // Arrange
        var serializerProvider = TestcontainersContext.Provider.GetRequiredService<MigrationBsonSerializerProvider>();

        // Act
        IBsonSerializer serializer = serializerProvider.GetSerializer(typeof(TestDocumentWithOneMigration));

        // Assert
        Assert.That(serializer, Is.TypeOf<MigrationDocumentSerializer<TestDocumentWithOneMigration>>());
    }

    [Test]
    public void If_type_is_not_assignable_to_document_Then_null_returned()
    {
        // Arrange
        var serializerProvider = TestcontainersContext.Provider.GetRequiredService<MigrationBsonSerializerProvider>();

        // Act
        IBsonSerializer serializer = serializerProvider.GetSerializer(typeof(TestClassNoMigration));

        // Assert
        Assert.That(serializer, Is.Null);
    }

    [Test]
    public void If_type_is_not_assignable_to_document_but_manually_added_Then_interceptor_created()
    {
        // Arrange
        var serializerProvider = TestcontainersContext.Provider.GetRequiredService<MigrationBsonSerializerProvider>();

        // Act
        IBsonSerializer serializer = serializerProvider.GetSerializer(typeof(TestClassWithTwoMigrationMiddleVersion));

        // Assert
        Assert.That(serializer, Is.TypeOf<MigrationReflexionSerializer<TestClassWithTwoMigrationMiddleVersion>>());
    }
}