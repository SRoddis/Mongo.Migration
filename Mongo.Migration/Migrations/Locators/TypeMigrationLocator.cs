using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationLocator : MigrationLocator
    {
        public override IDictionary<Type, IReadOnlyCollection<IMigration>> LoadMigrations()
        {
            var assemblies = GetAssemblies();
            var allMigrations = GetUniqueMigrations(assemblies);
            return GetMigrationDictionary(allMigrations);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(location);

            if (string.IsNullOrWhiteSpace(path))
                throw new DirectoryNotFoundException(ErrorTexts.AppDirNotFound);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var migrationAssemblies = Directory.GetFiles(path, "*.MongoMigrations.dll").Select(Assembly.LoadFile);

            assemblies.AddRange(migrationAssemblies);

            return assemblies;
        }

        private static IList<IMigration> GetUniqueMigrations(IEnumerable<Assembly> assemblies)
        {
            var migrationTypes =
                from assembly in assemblies
                from type in assembly.GetTypes()
                where typeof(IMigration).IsAssignableFrom(type) && !type.IsAbstract
                select type;

            return migrationTypes.Distinct().Select(t => (IMigration) Activator.CreateInstance(t)).ToList();
        }

        private static IDictionary<Type, IReadOnlyCollection<IMigration>> GetMigrationDictionary(
            IList<IMigration> migrations)
        {
            var dictonary = new Dictionary<Type, IReadOnlyCollection<IMigration>>();
            var types = from m in migrations select m.Type;

            foreach (var type in types)
            {
                if (dictonary.ContainsKey(type))
                    continue;

                var uniqueMigrations =
                    migrations.Where(m => m.Type == type).CheckForDuplicates().OrderBy(m => m.Version).ToList();
                dictonary.Add(type, uniqueMigrations);
            }

            return dictonary;
        }
    }
}