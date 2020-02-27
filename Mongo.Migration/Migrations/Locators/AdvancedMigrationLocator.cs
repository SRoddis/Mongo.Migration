using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    public abstract class AdvancedMigrationLocator : IAdvancedMigrationLocator
    {
        private IEnumerable<Assembly> _assemblies;
        
        protected IEnumerable<Assembly> Assemblies => _assemblies ?? (_assemblies = GetAssemblies());

        private IDictionary<Type, IReadOnlyCollection<IAdvancedMigration>> _migrations;

        protected IDictionary<Type, IReadOnlyCollection<IAdvancedMigration>> Migrations
        {
            get
            {
                if (_migrations == null)
                    Locate();
                
                if (_migrations.NullOrEmpty())
                    throw new NoMigrationsFoundException();

                return _migrations;
            }
            set { _migrations = value; }
        }

        public IEnumerable<IAdvancedMigration> GetMigrations(Type type)
        {
            IReadOnlyCollection<IAdvancedMigration> migrations;
            Migrations.TryGetValue(type, out migrations);

            return migrations;
        }

        public IEnumerable<IAdvancedMigration> GetMigrationsGt(Type type, string version)
        {
            var migrations = GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version.CompareTo(version) > 0)
                    .ToList();
        }

        public IEnumerable<IAdvancedMigration> GetMigrationsGtEq(Type type, string version)
        {
            var migrations = GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version.CompareTo(version) >= 0)
                    .ToList();
        }

        public DocumentVersion GetLatestVersion(Type type)
        {         
            var migrations = GetMigrations(type);

            return migrations.Max(m => m.Version);
        }

        public abstract void Locate();
        
        private static IEnumerable<Assembly> GetAssemblies()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.GetDirectoryName(location);

            if (string.IsNullOrWhiteSpace(path))
                throw new DirectoryNotFoundException(ErrorTexts.AppDirNotFound);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var migrationAssemblies = Directory.GetFiles(path, "*.MongoMigrations*.dll").Select(Assembly.LoadFile);

            assemblies.AddRange(migrationAssemblies);

            return assemblies;
        }
    }
}