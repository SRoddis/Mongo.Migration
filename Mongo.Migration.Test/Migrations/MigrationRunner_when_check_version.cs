using System;
using FluentAssertions;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations
{
    [TestFixture]
    internal class MigrationRunner_when_check_version : IntegrationTest
    {
        private IMigrationRunner _runner;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _runner = _components.Get<IMigrationRunner>();
        }

        [Test]
        public void When_document_has_current_version_Then_current_version_is_set()
        {
            // Arrange
            var document = new TestDocumentWithTwoMigrationMiddleVersion();

            // Act
            _runner.CheckVersion(document);

            // Assert
            document.Version.Should().Be("0.0.1");
        }

        [Test]
        public void When_document_has_highest_version_Then_highest_version_is_set()
        {
            // Arrange
            var document = new TestDocumentWithTwoMigrationHighestVersion();

            // Act
            _runner.CheckVersion(document);

            // Assert
            document.Version.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_version_that_should_not_be_Then_throw_exception()
        {
            // Arrange
            var document = new TestDocumentWithTwoMigrationHighestVersion { Version = "0.0.1"};

            // Act// Act
            Action checkAction = () => { _runner.CheckVersion(document); };

            // Assert
            checkAction.ShouldThrow<VersionViolationException>();
        }
    }
}