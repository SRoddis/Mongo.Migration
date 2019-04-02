using FluentAssertions;
using Mongo.Migration.Documents;
using Mongo.Migration.Services.Migration;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Mongo.Migration.Test.MongoDB
{
    [TestFixture]
    internal class MongoRegistrator_when_registrating : IntegrationTest
    {
        [Test]
        public void Then_serializer_is_registered()
        {
            // Arrange 
            var registrator = _components.Get<IMigrationStrategy>();

            // Act
            registrator.Migrate();

            // Arrange
            BsonSerializer.LookupSerializer<DocumentVersion>().ValueType.Should().Be(typeof(DocumentVersion));
        }
    }
}