﻿using System;
using System.Linq;
using FluentAssertions;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations;
using Mongo.Migration.Test.TestDoubles;
using MongoDB.Bson;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations
{
    [TestFixture]
    internal class MigrationRunner_when_migrating_up : IntegrationTest
    {
        private IMigrationRunner<IDocument> _runner;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _runner = _components.Get<IMigrationRunner<IDocument>>();
        }

        [Test]
        public void When_migrate_up_the_lowest_version_Then_all_migrations_are_used()
        {
            // Arrange
            BsonDocument document = new BsonDocument
            {
                {"Version", "0.0.0"},
                {"Dors", 3}
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_no_version_Then_all_migrations_are_used()
        {
            // Arrange
            BsonDocument document = new BsonDocument
            {
                {"Dors", 3}
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_current_version_Then_nothing_happens()
        {
            // Arrange
            BsonDocument document = new BsonDocument
            {
                {"Version", "0.0.2"},
                {"Door", 3}
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }
    }
}