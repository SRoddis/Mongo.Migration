using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Extensions;

using NLog;

namespace Mongo.Migration.Migrations.Locators
{
    public abstract class MigrationLocator<TMigrationType> : IMigrationLocator<TMigrationType>
        where TMigrationType : class, IMigration
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IEnumerable<Assembly> _assemblies;

        private IDictionary<Type, IReadOnlyCollection<TMigrationType>> _migrations;

        protected IEnumerable<Assembly> Assemblies => this._assemblies ??= GetAssemblies();

        protected virtual IDictionary<Type, IReadOnlyCollection<TMigrationType>> Migrations
        {
            get
            {
                if (this._migrations == null)
                {
                    this.Locate();
                }

                if (this._migrations.NullOrEmpty())
                {
                    this._logger.Debug(new NoMigrationsFoundException());
                }

                return this._migrations;
            }
            set => this._migrations = value;
        }

        public IEnumerable<TMigrationType> GetMigrations(Type type)
        {
            IReadOnlyCollection<TMigrationType> migrations;
            this.Migrations.TryGetValue(type, out migrations);

            return migrations ?? Enumerable.Empty<TMigrationType>();
        }

        public IEnumerable<TMigrationType> GetMigrationsFromTo(Type type, DocumentVersion version, DocumentVersion otherVersion)
        {
            var migrations = this.GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version > version)
                    .Where(m => m.Version <= otherVersion)
                    .ToList();
        }

        public IEnumerable<TMigrationType> GetMigrationsGt(Type type, DocumentVersion version)
        {
            var migrations = this.GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version > version)
                    .ToList();
        }

        public IEnumerable<TMigrationType> GetMigrationsGtEq(Type type, DocumentVersion version)
        {
            var migrations = this.GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version >= version)
                    .ToList();
        }

        public DocumentVersion GetLatestVersion(Type type)
        {
            var migrations = this.GetMigrations(type);

            if (migrations == null || !migrations.Any())
            {
                return DocumentVersion.Default();
            }

            return migrations.Max(m => m.Version);
        }

        public abstract void Locate();

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.GetDirectoryName(location);

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DirectoryNotFoundException(ErrorTexts.AppDirNotFound);
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var migrationAssemblies = Directory.GetFiles(path, "*.MongoMigrations*.dll").Select(Assembly.LoadFile);

            assemblies.AddRange(migrationAssemblies);

            return assemblies;
        }
    }
}