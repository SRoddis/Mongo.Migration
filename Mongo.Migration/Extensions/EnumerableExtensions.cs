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

        internal static IEnumerable<IAdvancedMigration> CheckForDuplicates(this IEnumerable<IAdvancedMigration> list)
        {
            var uniqueHashes = new HashSet<string>();
            foreach (var element in list)
            {
                var version = element.Version.ToString();
                var typeName = element.GetType().Name;
                
                if (uniqueHashes.Add(typeName+version))
                    continue;

                throw new DuplicateVersionException(typeName, element.Version);
            }

            return list;
        }
        
        internal static IDictionary<Type, IReadOnlyCollection<IAdvancedMigration>> ToMigrationDictionary(
            this IEnumerable<IAdvancedMigration> migrations)
        {
            var dictonary = new Dictionary<Type, IReadOnlyCollection<IAdvancedMigration>>();
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