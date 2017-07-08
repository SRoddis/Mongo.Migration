using System;
using System.Linq;
using FluentAssertions;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Locators
{
    [TestFixture]
    public class AttributeMigrationLocator_when_locate
    {
        [Test]
        public void Then_find_all_migrations()
        {
            // Arrange
            var locator = new AttributeMigrationLocator();

            // Act
            var result = locator.GetMigrations(typeof(TestDocument));

            // Assert
            result.Count().Should().Be(1);
        }
    }
}