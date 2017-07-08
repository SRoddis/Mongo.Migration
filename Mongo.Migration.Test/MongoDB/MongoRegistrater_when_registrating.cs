using FluentAssertions;
using Mongo.Migration.Models;
using Mongo.Migration.Models.Serializers;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Services.MongoDB;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Mongo.Migration.Test.MongoDB
{
    [TestFixture]
    public class MongoRegistrater_when_registrating
    {
        [Test]
        public void Then_serializer_is_registered()
        {
            // Arrange 
            var serializer = new DocumentVersionSerializer();
            var provider = new MigrationInterceptorProvider();
            var registrater = new MongoRegistrater(serializer, provider);

            // Act
            registrater.Registrate();

            // Arrange
            BsonSerializer.LookupSerializer<DocumentVersion>().ValueType.Should().Be(typeof(DocumentVersion));
        }
    }
}