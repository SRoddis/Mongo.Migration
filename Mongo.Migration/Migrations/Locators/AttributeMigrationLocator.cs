using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Attributes;

namespace Mongo.Migration.Migrations.Locators
{
    internal class AttributeMigrationLocator : MigrationLocator
    {
        public override void LoadMigrations()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            AppendMigrationAssemblies(assemblies);

            var migrationTypes =
                from a in assemblies
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(MigrationMaker), true)
                where attributes != null && attributes.Length > 0
                select new {Type = t, Attributes = attributes.Cast<MigrationMaker>()};

            var dictonary = new Dictionary<Type, IReadOnlyCollection<IMigration>>();

            var allMigrations = migrationTypes.Select(t => (IMigration) Activator.CreateInstance(t.Type))
                .OrderBy(m => m.Version);

            var types = allMigrations.Select(m => m.Type).Distinct();
            foreach (var type in types)
            {
                var migrations = allMigrations.Where(m => m.Type == type).CheckForDuplicates();
                dictonary.Add(type, migrations.ToList());
            }

            _migrations =  dictonary;
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