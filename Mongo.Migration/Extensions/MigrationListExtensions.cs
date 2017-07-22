using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Migrations;

namespace Mongo.Migration.Extensions
{
    internal static class MigrationListExtensions
    {
        public static IDictionary<Type, IReadOnlyCollection<IMigration>> ToMigrationDictionary(
            this IList<IMigration> migrations)
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