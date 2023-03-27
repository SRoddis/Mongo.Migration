﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private IDictionary<Type, IReadOnlyCollection<TMigrationType>> _migrations;

        protected virtual IDictionary<Type, IReadOnlyCollection<TMigrationType>> Migrations
        {
            get
            {
                if (_migrations == null)
                {
                    Locate();
                }

                if (_migrations.NullOrEmpty())
                {
                    _logger.Info(new NoMigrationsFoundException());
                }

                return _migrations;
            }
            set => _migrations = value;
        }

        public IEnumerable<TMigrationType> GetMigrations(Type type)
        {
            Migrations.TryGetValue(type, out var migrations);
            return migrations ?? Enumerable.Empty<TMigrationType>();
        }

        public IEnumerable<TMigrationType> GetMigrationsFromTo(Type type, DocumentVersion version, DocumentVersion otherVersion)
        {
            var migrations = GetMigrations(type);
            return migrations.Where(m => m.Version > version && m.Version <= otherVersion);
        }

        public IEnumerable<TMigrationType> GetMigrationsGt(Type type, DocumentVersion version)
        {
            var migrations = GetMigrations(type);
            return migrations.Where(m => m.Version > version);
        }

        public IEnumerable<TMigrationType> GetMigrationsGtEq(Type type, DocumentVersion version)
        {
            var migrations = GetMigrations(type);
            return migrations.Where(m => m.Version >= version);
        }

        public DocumentVersion GetLatestVersion(Type type)
        {
            var migrations = GetMigrations(type);

            if (migrations == null || !migrations.Any())
            {
                return DocumentVersion.Default();
            }

            return migrations.Max(m => m.Version);
        }

        public abstract void Locate();
    }
}