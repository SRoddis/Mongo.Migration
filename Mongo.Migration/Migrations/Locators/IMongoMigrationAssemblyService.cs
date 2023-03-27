using System.Collections.Generic;
using System.Reflection;

namespace Mongo.Migration.Migrations.Locators;

public interface IMongoMigrationAssemblyService
{
    IEnumerable<Assembly> GetAssemblies();
}