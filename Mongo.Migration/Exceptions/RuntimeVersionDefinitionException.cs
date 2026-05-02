using Mongo.Migration.Documents;

namespace Mongo.Migration.Exceptions;
public class RuntimeVersionDefinitionException : Exception
{
    internal RuntimeVersionDefinitionException(
        Type type,
        DocumentVersion addMigrationRuntimeVersionDefinition,
        DocumentVersion attributeRuntimeVersionDefinition)
        : base($"Migrated type {type.Name} runtime version definition conflict: AddMigration<{type.Name}>@{addMigrationRuntimeVersionDefinition} != RuntimeVersionAttribute({attributeRuntimeVersionDefinition})")
    {

    }
}
