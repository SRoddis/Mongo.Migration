using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Database
{
    [TestFixture]
    internal class DatabaseMigrationRunnerWhenMigratingDownTest : DatabaseIntegrationTest
    {
        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public void When_database_has_migrations_Then_down_all_migrations()
        {
            OnSetUp(DocumentVersion.Default());
            using var scoped = ServiceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDatabaseMigrationRunner>();
            
            InsertMigrations(
                new DatabaseMigration[]
                {
                    new TestDatabaseMigration_0_0_1(),
                    new TestDatabaseMigration_0_0_2(),
                    new TestDatabaseMigration_0_0_3()
                });

            runner.Run(Db);

            var migrations = GetMigrationHistory();
            migrations.Should().BeEmpty();
        }

        [Test]
        public void When_database_has_migrations_Then_down_to_selected_migration()
        {
            OnSetUp(new DocumentVersion("0.0.1"));
            using var scoped = ServiceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDatabaseMigrationRunner>();
            
            InsertMigrations(
                new DatabaseMigration[]
                {
                    new TestDatabaseMigration_0_0_1(),
                    new TestDatabaseMigration_0_0_2(),
                    new TestDatabaseMigration_0_0_3()
                });

            runner.Run(Db);

            var migrations = GetMigrationHistory();
            migrations.Should().NotBeEmpty();
            migrations.Should().OnlyContain(m => m.Version == "0.0.1");
        }
    }
}