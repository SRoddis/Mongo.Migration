using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Extensions;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Migrations.Locators
{
    public abstract class MigrationLocator : IMigrationLocator
    {
        private IEnumerable<Assembly> _assemblies;
        
        protected IEnumerable<Assembly> Assemblies => _assemblies ?? (_assemblies = GetAssemblies());

        private IDictionary<Type, IReadOnlyCollection<IMigration>> _migrations;

        protected IDictionary<Type, IReadOnlyCollection<IMigration>> Migrations
        {
            get
            {
                if (_migrations == null)
                    LoadMigrations();
                
                if (_migrations.NullOrEmpty())
                    throw new NoMigrationsFoundException();

                return _migrations;
            }
            set { _migrations = value; }
        }

        public IEnumerable<IMigration> GetMigrations(Type type)
        {
            IReadOnlyCollection<IMigration> migrations;
            Migrations.TryGetValue(type, out migrations);

            return migrations;
        }

        public IEnumerable<IMigration> GetMigrationsGt(Type type, DocumentVersion version)
        {
            var migrations = GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version > version)
                    .ToList();
        }

        public IEnumerable<IMigration> GetMigrationsGtEq(Type type, DocumentVersion version)
        {
            var migrations = GetMigrations(type);

            return
                migrations
                    .Where(m => m.Version >= version)
                    .ToList();
        }

        public DocumentVersion GetLatestVersion(Type type)
        {         
            var migrations = GetMigrations(type);

            return migrations.Max(m => m.Version);
        }

        public abstract void LoadMigrations();
        
        private static IEnumerable<Assembly> GetAssemblies()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var path = Path.GetDirectoryName(location);

            if (string.IsNullOrWhiteSpace(path))
                throw new DirectoryNotFoundException(ErrorTexts.AppDirNotFound);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var migrationAssemblies = Directory.GetFiles(path, "*.MongoMigrations.dll").Select(Assembly.LoadFile);

            assemblies.AddRange(migrationAssemblies);

            return assemblies;
        }
    }
}