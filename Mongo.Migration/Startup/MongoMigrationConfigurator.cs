using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using System.Reflection;

namespace Mongo.Migration.Startup;

public class MongoMigrationConfigurator
{
    internal MongoMigrationSettings MongoMigrationSettings { get; }
    internal MongoMigrationStartupSettings MongoMigrationStartupSettings { get; }
    internal Dictionary<Type, DocumentVersion> RuntimeMigrationDictionary { get; }
    
    internal MongoMigrationConfigurator()
    {
        MongoMigrationSettings = new MongoMigrationSettings();
        MongoMigrationStartupSettings = new MongoMigrationStartupSettings();
        RuntimeMigrationDictionary = [];
    }

    public MongoMigrationConfigurator SetVersionFieldName(string fieldName)
    {
        MongoMigrationSettings.VersionFieldName = fieldName;
        return this;
    }

    public MongoMigrationConfigurator AddRuntimeDocumentMigration()
    {
        MongoMigrationStartupSettings.RuntimeMigrationEnabled = true;
        return this;
    }

    public MongoMigrationConfigurator AddStartupDocumentMigration()
    {
        MongoMigrationStartupSettings.StartupDocumentMigrationEnabled = true;
        return this;
    }

    public MongoMigrationConfigurator AddDocumentMigratedType<T>(DocumentVersion runtimeVersion)
    {
        Type t = typeof(T);
        RuntimeVersionAttribute? runtimeVersionAttribute = t.GetCustomAttributes<RuntimeVersionAttribute>(true)
            .FirstOrDefault();

        if (runtimeVersionAttribute is null || runtimeVersionAttribute.Version == runtimeVersion)
        {
            RuntimeMigrationDictionary.Add(t, runtimeVersion);
        }
        else
        {
            throw new RuntimeVersionDefinitionException(t, runtimeVersion, runtimeVersionAttribute.Version);
        }

        return this;
    }

    public MongoMigrationConfigurator AddDocumentMigratedType<T>(string runtimeVersion)
    {
        return AddDocumentMigratedType<T>(DocumentVersion.Parse(runtimeVersion.AsSpan()));
    }

    public MongoMigrationConfigurator AddDatabaseMigration()
    {
        MongoMigrationStartupSettings.DatabaseMigrationEnabled = true;
        return this;
    }

    internal void AddAllMigrationsIfNothingWasAdded()
    {
        if (MongoMigrationStartupSettings is { RuntimeMigrationEnabled: false, StartupDocumentMigrationEnabled: false, DatabaseMigrationEnabled: false })
        {
            AddRuntimeDocumentMigration();
            AddDatabaseMigration();
            AddStartupDocumentMigration();
        }
    }
}