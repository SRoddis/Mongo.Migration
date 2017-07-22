using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationLocator : MigrationLocator
    {
        public override IDictionary<Type, IReadOnlyCollection<IMigration>> LoadMigrations()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            AppendMigrationAssemblies(assemblies);

            var migrationTypes =
                from a in assemblies
                from t in a.GetTypes()
                where typeof(IMigration).IsAssignableFrom(t) && !t.IsAbstract
                select t;

            var dictonary = new Dictionary<Type, IReadOnlyCollection<IMigration>>();

            var allMigrations = migrationTypes.Select(t => (IMigration) Activator.CreateInstance(t))
                .OrderBy(m => m.Version);

            var types = allMigrations.Select(m => m.Type).Distinct();
            foreach (var type in types)
            {
                var migrations = allMigrations.Where(m => m.Type == type).CheckForDuplicates();
                dictonary.Add(type, migrations.ToList());
            }

            return dictonary;
        }

        private static void AppendMigrationAssemblies(List<Assembly> assemblies)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(location);

            foreach (var dll in Directory.GetFiles(path, "*.MongoMigrations.dll"))
                assemblies.Add(Assembly.LoadFile(dll));
        }
    }
}