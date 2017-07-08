using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    public abstract class MigrationLocator : IMigrationLocator
    {
        private IDictionary<Type, IEnumerable<IMigration>> _migrations;

        protected IDictionary<Type, IEnumerable<IMigration>> Migrations
        {
            get
            {
                if (_migrations == null)
                    _migrations =
                        LoadMigrations();

                if (_migrations.NullOrEmpty())
                    throw new NoMigrationsFoundException();

                return _migrations;
            }
        }

        public IEnumerable<IMigration> GetMigrations(Type type)
        {
            IEnumerable<IMigration> migrations;
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

        protected abstract IDictionary<Type, IEnumerable<IMigration>> LoadMigrations();
    }
}