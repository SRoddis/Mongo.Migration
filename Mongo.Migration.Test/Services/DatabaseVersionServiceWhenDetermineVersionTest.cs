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
        private IDatabaseVersionService _service;

        protected override void OnSetUp(DocumentVersion version)
        {
            base.OnSetUp(version);

            _service = ServiceProvider.GetRequiredService<IDatabaseVersionService>();
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public void When_project_has_migrations_Then_get_latest_version()
        {
            OnSetUp(DocumentVersion.Empty());

            var migrationVersion = _service.GetCurrentOrLatestMigrationVersion();

            migrationVersion.ToString().Should().Be("0.0.3");
        }

        [Test]
        public void When_version_set_on_startup_Then_use_startup_version()
        {
            OnSetUp(new DocumentVersion(0, 0, 2));

            var migrationVersion = _service.GetCurrentOrLatestMigrationVersion();

            migrationVersion.ToString().Should().Be("0.0.2");
        }
    }
}