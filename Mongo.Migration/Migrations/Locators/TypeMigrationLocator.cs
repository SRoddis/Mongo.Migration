using Microsoft.Extensions.Logging;
using Mongo.Migration.Migrations.Document;

namespace Mongo.Migration.Migrations.Locators;

internal class TypeMigrationLocator : MigrationLocator<IDocumentMigration>
{
    public TypeMigrationLocator(ILogger<TypeMigrationLocator> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider) { }
}