using System;
using FluentAssertions;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services.Startup.Static;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services.Initializers
{
    [TestFixture]
    public class MongoMigration_when_initialize
    {
        [TearDown]
        public void TearDown()
        {
            Migration.Services.Startup.Static.MongoMigration.Reset();
        }

        [Test]
        public void When_inizialize_twice_Then_throw_exception()
        {
            // Arrange
            Migration.Services.Startup.Static.MongoMigration.Initialize();

            // Act
            Action comparison = Migration.Services.Startup.Static.MongoMigration.Initialize;

            // Assert
            comparison.ShouldThrow<AlreadyInitializedException>();
        }
    }
}