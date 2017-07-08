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
        private IList<IMigration> _migrations;

        protected IList<IMigration> Migrations
        {
            get
            {
                if (_migrations == null)
                    _migrations = LoadMigrations().OrderBy(m => m.Type).CheckForDuplicates().ToList();

                if (_migrations.NullOrEmpty())
                    throw new NoMigrationsFoundException();

                return _migrations;
            }
        }

        public IEnumerable<IMigration> GetMigrations(Type type)
        {
            return Migrations.Where(m => m.Type == type).OrderBy(m => m.Version).ToList();
        }

        public IEnumerable<IMigration> GetMigrationsGt(Type type, DocumentVersion version)
        {
            return
                Migrations.Where(m => m.Type == type)
                    .OrderBy(m => m.Version)
                    .Where(m => m.Version > version)
                    .ToList();
        }

        public IEnumerable<IMigration> GetMigrationsLte(Type type, DocumentVersion version)
        {
            return
                Migrations.Where(m => m.Type == type)
                    .OrderByDescending(m => m.Version)
                    .Where(m => m.Version <= version)
                    .ToList();
        }

        public DocumentVersion GetLatestVersion(Type type)
        {
            return Migrations.Where(m => m.Type == type).Max(m => m.Version);
        }

        protected abstract IEnumerable<IMigration> LoadMigrations();
    }
}