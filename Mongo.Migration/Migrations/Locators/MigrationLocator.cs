using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators;

public abstract class MigrationLocator<TMigrationType> : IMigrationLocator<TMigrationType>
    where TMigrationType : class, IMigration
{
    private readonly ILogger<MigrationLocator<TMigrationType>> _logger;

    private readonly IServiceProvider _serviceProvider;

    private readonly Lazy<IDictionary<Type, ReadOnlyCollection<TMigrationType>>> _lazyMigrations;

    protected MigrationLocator(ILogger<MigrationLocator<TMigrationType>> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _lazyMigrations = new Lazy<IDictionary<Type, ReadOnlyCollection<TMigrationType>>>(
            LoadMigrations,
            LazyThreadSafetyMode.PublicationOnly);
    }

    protected virtual IDictionary<Type, ReadOnlyCollection<TMigrationType>> Migrations
        => _lazyMigrations.Value;

    public IReadOnlyCollection<TMigrationType> GetMigrations(Type type)
    {
        if(Migrations.TryGetValue(type, out var migrations))
        {
            return migrations;
        }

        return [];
    }

    public IEnumerable<TMigrationType> GetMigrationsFromTo(Type type, DocumentVersion version, DocumentVersion otherVersion)
    {
        return GetMigrations(type)
            .Where(m => m.Version > version && m.Version <= otherVersion);
    }

    public IEnumerable<TMigrationType> GetMigrationsFromToDown(Type type, DocumentVersion version, DocumentVersion otherVersion)
    {
        return GetMigrations(type)
            .Where(m => m.Version <= version && m.Version > otherVersion)
            .Reverse();
    }

    public DocumentVersion GetLatestVersion(Type type)
    {
        var migrations = GetMigrations(type);

        return migrations.Count > 0
            ? migrations.Max(m => m.Version)
            : DocumentVersion.Default;
    }

    public void Initialize()
    {
        if (!_lazyMigrations.IsValueCreated)
        {
            var migrations = _lazyMigrations.Value;
            _logger.LogDebug("Migration locator initialized and loaded {Count} migrations", migrations.Count);
        }
    }

    private IDictionary<Type, ReadOnlyCollection<TMigrationType>> LoadMigrations()
    {
        Type migrationType = typeof(TMigrationType);

        List<Assembly> assemblies = GetAssemblies();

        var migrationTypes = assemblies
            .SelectMany(a => a.GetExportedTypes())
            .Where(type => !type.IsAbstract && migrationType.IsAssignableFrom(type))
            .DistinctBy(t => t.AssemblyQualifiedName)
            .Select(GetMigrationInstance)
            .ToMigrationDictionary();

        _logger.LogDebug("{Count} {MigrationType} migrations found", migrationTypes.Count, migrationType.Name);

        return migrationTypes;
    }

    private static List<Assembly> GetAssemblies()
    {
        var location = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.GetDirectoryName(location);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new DirectoryNotFoundException(location);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var migrationAssemblies = Directory.GetFiles(path, "*.MongoMigrations*.dll").Select(Assembly.LoadFile);

        assemblies.AddRange(migrationAssemblies);

        return assemblies;
    }

    private TMigrationType GetMigrationInstance(Type type)
    {
        return ActivatorUtilities.CreateInstance(_serviceProvider, type) as TMigrationType
               ?? throw new InvalidOperationException($"Cannot create {type} migration");
    }
}