using System;
using System.Linq;
using FluentAssertions;
using Mongo.Migration.Migrations;
using Mongo.Migration.Test.TestDoubles;
using MongoDB.Bson;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations
{
    [TestFixture]
    internal class MigrationRunner_when_migrating_down : IntegrationTest
    {
        private IMigrationRunner _runner;

        [SetUp]
        public void SetUp()
        {
            base.OnSetUp();
            
            _runner = _components.Get<IMigrationRunner>();
        }

        [TearDown]
        public void TearDown()
        {
            this.Dispose();
        }

        [Test]
        public void When_migrating_down_Then_all_migrations_are_used()
        {
            // Arrange
            BsonDocument document = new BsonDocument
            {
                {"Version", "0.0.2"},
                {"Door", 3}
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigration), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Dors");
            document.Values.ToList()[0].AsString.Should().Be("0.0.0");
        }

        [Test]
        public void When_document_has_Then_all_migrations_are_used_to_that_version()
        {
            // Arrange
            // Arrange
            BsonDocument document = new BsonDocument
            {
                {"Version", "0.0.2"},
                {"Door", 3}
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigrationMiddleVersion), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Doors");
            document.Values.ToList()[0].AsString.Should().Be("0.0.1");
        }
    }
}