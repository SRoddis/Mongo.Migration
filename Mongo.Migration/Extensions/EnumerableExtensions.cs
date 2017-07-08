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
    }
}