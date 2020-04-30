using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Test.TestDoubles;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database
{
    [TestFixture]
    internal class DatabaseMigrationRunner_when_migrating_up : DatabaseIntegrationTest
    {
        private IDatabaseMigrationRunner _runner;


        [SetUp]
        public void SetUp()
        {
            base.OnSetUp(DocumentVersion.Empty());

            _runner = _components.Get<IDatabaseMigrationRunner>();
        }

        [TearDown]
        public void TearDown()
        {
            this.Dispose();
        }

        [Test]
        public void When_database_has_no_migrations_Then_all_migrations_are_used()
        {
            // Act
            _runner.Run(_db);

            // Assert
            var migrations = GetMigrationHistory();
            migrations.Should().NotBeEmpty();
            migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
            migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
            migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
        }

        [Test]
        public void When_database_has_migrations_Then_latest_migrations_are_used()
        {
            // Assert
            InsertMigrations(new DatabaseMigration[] { new TestDatabaseMigration_0_0_1(), new TestDatabaseMigration_0_0_2() });
           
            // Act
            _runner.Run(_db);

            // Assert
            var migrations = GetMigrationHistory();
            migrations.Should().NotBeEmpty();
            migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
        }

        [Test]
        public void When_database_has_latest_version_Then_nothing_happens()
        {
            // Assert
            InsertMigrations(new DatabaseMigration[] { new TestDatabaseMigration_0_0_1(), new TestDatabaseMigration_0_0_2(), new TestDatabaseMigration_0_0_3() });

            // Act
            _runner.Run(_db);

            // Assert
            var migrations = GetMigrationHistory();
            migrations.Should().NotBeEmpty();
            migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
            migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
            migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
        }
    }
}