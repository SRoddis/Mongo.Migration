using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Services;
using Mongo.Migration.Test.Migrations.Database;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services
{
    [TestFixture]
    internal class DatabaseVersionServiceWhenDetermineVersionTest : DatabaseIntegrationTest
    {
        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public void When_project_has_migrations_Then_get_latest_version()
        {
            OnSetUp(DocumentVersion.Empty());
            using var scoped = ServiceProvider.CreateScope();
            var service = scoped.ServiceProvider.GetRequiredService<IDatabaseVersionService>();

            var migrationVersion = service.GetCurrentOrLatestMigrationVersion();

            migrationVersion.ToString().Should().Be("0.0.3");
        }

        [Test]
        public void When_version_set_on_startup_Then_use_startup_version()
        {
            OnSetUp(new DocumentVersion(0, 0, 2));
            using var scoped = ServiceProvider.CreateScope();
            var service = scoped.ServiceProvider.GetRequiredService<IDatabaseVersionService>();
            
            var migrationVersion = service.GetCurrentOrLatestMigrationVersion();

            migrationVersion.ToString().Should().Be("0.0.2");
        }
    }
}