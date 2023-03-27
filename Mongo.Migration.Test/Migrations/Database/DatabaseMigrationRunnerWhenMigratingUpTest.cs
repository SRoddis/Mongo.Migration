using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database
{
    [TestFixture]
    internal class DatabaseMigrationRunnerWhenMigratingUpTest : DatabaseIntegrationTest
    {
        [SetUp]
        public void SetUp()
        {
            base.OnSetUp(DocumentVersion.Empty());
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public void When_database_has_no_migrations_Then_all_migrations_are_used()
        {
            using var scoped = ServiceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDatabaseMigrationRunner>();
            
            runner.Run(Db);

            var migrations = GetMigrationHistory();
            migrations.Should().NotBeEmpty();
            migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
            migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
            migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
        }

        [Test]
        public void When_database_has_migrations_Then_latest_migrations_are_used()
        {
            InsertMigrations(new DatabaseMigration[] { new TestDatabaseMigration_0_0_1(), new TestDatabaseMigration_0_0_2() });
            using var scoped = ServiceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDatabaseMigrationRunner>();
            
            runner.Run(Db);

            var migrations = GetMigrationHistory();
            migrations.Should().NotBeEmpty();
            migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
        }

        [Test]
        public void When_database_has_latest_version_Then_nothing_happens()
        {
            InsertMigrations(
                new DatabaseMigration[] { new TestDatabaseMigration_0_0_1(), new TestDatabaseMigration_0_0_2(), new TestDatabaseMigration_0_0_3() });
            
            using var scoped = ServiceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDatabaseMigrationRunner>();
            
            runner.Run(Db);

            var migrations = GetMigrationHistory();
            migrations.Should().NotBeEmpty();
            migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
            migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
            migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
        }
    }
}