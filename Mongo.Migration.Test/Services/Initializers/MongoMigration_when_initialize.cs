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
            Startup.Static.MongoMigration.Reset();
        }

        [Test]
        public void When_inizialize_twice_Then_throw_exception()
        {
            // Arrange
            Startup.Static.MongoMigration.MigrationOnDeserialization();

            // Act
            Action comparison = Startup.Static.MongoMigration.MigrationOnDeserialization;

            // Assert
            comparison.ShouldThrow<AlreadyInitializedException>();
        }
    }
}