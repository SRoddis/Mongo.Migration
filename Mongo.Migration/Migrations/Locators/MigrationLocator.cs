using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Extensions;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Migrations.Locators
{
    public abstract class MigrationLocator : IMigrationLocator
    {
        private IDictionary<Type, IReadOnlyCollection<IMigration>> _migrations;

        protected IDictionary<Type, IReadOnlyCollection<IMigration>> Migrations
        {
            get
            {
                if (_migrations == null)
                    _migrations = LoadMigrations();

                if (_migrations.NullOrEmpty())
                    throw new NoMigrationsFoundException();

                return _migrations;
            }
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

        public IEnumerable<IMigration> GetMigrationsGtAndEquel(Type type, DocumentVersion version)
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

        public abstract IDictionary<Type, IReadOnlyCollection<IMigration>> LoadMigrations();
    }
}