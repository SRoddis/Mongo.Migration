using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Services;
using Mongo.Migration.Tests.Migrations.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Services;

[TestFixture]
internal class DatabaseVersionServiceWhenDetermineVersion : DatabaseIntegrationTest
{
    [Test]
    public async Task WhenProjectHasMigrationsThenGetLatestVersion()
    {
        // Arrange 
        await OnSetUpAsync();
        IDatabaseVersionService service = TestcontainersContext.Provider.GetRequiredService<IDatabaseVersionService>();

        // Act
        var migrationVersion = service.GetLatestMigrationVersion();

        // Assert
        Assert.That(migrationVersion.ToString(), Is.EqualTo("0.0.3"));
    }
}