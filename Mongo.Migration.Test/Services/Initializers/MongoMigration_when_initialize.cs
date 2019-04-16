using System;
using FluentAssertions;
using Mongo.Migration.Exceptions;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services.Initializers
{
    [TestFixture]
    public class MongoMigration_when_initialize
    {
        [TearDown]
        public void TearDown()
        {
            Startup.Static.MongoMigrationClient.Reset();
        }

        [Test]
        public void When_inizialize_twice_Then_throw_exception()
        {
            // Arrange
            Startup.Static.MongoMigrationClient.MigrationOnDeserialization();

            // Act
            Action comparison = Startup.Static.MongoMigrationClient.MigrationOnDeserialization;

            // Assert
            comparison.ShouldThrow<AlreadyInitializedException>();
        }
    }
}