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
        
        internal static IDictionary<Type, IReadOnlyCollection<TMigrationType>> ToMigrationDictionary<TMigrationType>(
            this IEnumerable<TMigrationType> migrations)
            where TMigrationType : class, IMigration
        {
            var dictonary = new Dictionary<Type, IReadOnlyCollection<TMigrationType>>();
            var types = (from m in migrations select m.Type).Distinct();

            foreach (var type in types)
            {
                if (dictonary.ContainsKey(type))
                {
                    continue;
                }

                var uniqueMigrations = migrations.Where(m => m.Type == type).CheckForDuplicates().OrderBy(m => m.Version).ToList();
                dictonary.Add(type, uniqueMigrations);
            }

            return dictonary;
        }
        
        private static IEnumerable<TMigrationType> CheckForDuplicates<TMigrationType>(this IEnumerable<TMigrationType> list)
            where TMigrationType : class, IMigration
        {
            var uniqueHashes = new HashSet<string>();
            foreach (var element in list)
            {
                var version = element.Version.ToString();
                if (uniqueHashes.Add(version))
                {
                    continue;
                }

                var typeName = element.GetType().Name;
                throw new DuplicateVersionException(typeName, element.Version);
            }

            return list;
        }
    }
}