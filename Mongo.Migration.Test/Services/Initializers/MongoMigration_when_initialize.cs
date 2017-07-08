using System;
using FluentAssertions;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services.Initializers;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services.Initializers
{
    [TestFixture]
    public class MongoMigration_when_initialize
    {
        [Test]
        public void When_inizialize_twice_Then_throw_exception()
        {
            // Arrange
            MongoMigration.Initialize();

            // Act
            Action comparison = MongoMigration.Initialize;

            // Assert
            comparison.ShouldThrow<AlreadyInitializedException>();
        }
    }
}