using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations;

namespace Mongo.Migration.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static bool NullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        internal static IEnumerable<IMigration> CheckForDuplicates(this IEnumerable<IMigration> list)
        {
            var uniqueHashes = new HashSet<string>();
            foreach (var element in list)
            {
                var version = element.Version.ToString();
                if (uniqueHashes.Add(version))
                    continue;

                var typeName = element.GetType().Name;
                throw new DuplicateVersionException(typeName, element.Version);
            }

            return list;
        }
        
        internal static IDictionary<Type, IReadOnlyCollection<IMigration>> ToMigrationDictionary(
            this IEnumerable<IMigration> migrations)
        {
            var dictonary = new Dictionary<Type, IReadOnlyCollection<IMigration>>();
            var list = migrations.ToList();
            var types = (from m in list select m.Type).Distinct();

            foreach (var type in types)
            {
                if (dictonary.ContainsKey(type))
                    continue;

                var uniqueMigrations =
                    list.Where(m => m.Type == type).CheckForDuplicates().OrderBy(m => m.Version).ToList();
                dictonary.Add(type, uniqueMigrations);
            }

            return dictonary;
        }
    }
}