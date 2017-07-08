using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Attributes;

namespace Mongo.Migration.Migrations.Locators
{
    internal class AttributeMigrationLocator : MigrationLocator
    {
        protected override IDictionary<Type, IEnumerable<IMigration>> LoadMigrations()
        {
            var migrationTypes =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(MigrationMaker), true)
                where attributes != null && attributes.Length > 0
                select new {Type = t, Attributes = attributes.Cast<MigrationMaker>()};

            //TODO: Throw exception if type is not IMigration

            var dictonary = new Dictionary<Type, IEnumerable<IMigration>>();

            var allMigrations = migrationTypes.Select(t => (IMigration) Activator.CreateInstance(t.Type))
                .OrderBy(m => m.Version);

            var types = allMigrations.Select(m => m.Type).Distinct();
            foreach (var type in types)
            {
                var migrations = allMigrations.Where(m => m.Type == type).CheckForDuplicates();
                dictonary.Add(type, migrations);
            }

            return dictonary;
        }
    }
}