using System.Collections.Frozen;
using System.Collections.ObjectModel;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations;

namespace Mongo.Migration.Extensions;

internal static class EnumerableExtensions
{
    private static IEnumerable<TMigrationType> CheckForDuplicates<TMigrationType>(this IEnumerable<TMigrationType> list)
        where TMigrationType : IMigration
    {
        var uniqueHashes = new HashSet<DocumentVersion>();
        foreach (var element in list)
        {
            if (uniqueHashes.Add(element.Version))
            {
                yield return element;
            }
            else
            {
                throw new DuplicateVersionException(element.GetType().Name, element.Version.ToString());
            }
        }
    }

    internal static IDictionary<Type, ReadOnlyCollection<TMigrationType>> ToMigrationDictionary<TMigrationType>(
        this IEnumerable<TMigrationType> migrations)
        where TMigrationType : IMigration
    {
        return migrations
            .GroupBy(m => m.Type)
            .ToFrozenDictionary(
                g => g.Key,
                g => g
                    .CheckForDuplicates()
                    .OrderBy(m => m.Version).ToList().AsReadOnly());
    }
}