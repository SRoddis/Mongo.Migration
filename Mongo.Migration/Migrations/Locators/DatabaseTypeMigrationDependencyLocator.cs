using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Startup;

namespace Mongo.Migration.Migrations.Locators
{
    internal class DatabaseTypeMigrationDependencyLocator : TypeMigrationDependencyLocator<IDatabaseMigration>, IDatabaseTypeMigrationDependencyLocator
    {
        private IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> _migrations;

        protected override IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> Migrations
        {
            get
            {
                if (_migrations == null)
                {
                    Locate();
                }

                return _migrations;
            }
            set => _migrations = value;
        }

        public DatabaseTypeMigrationDependencyLocator(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
        }
    }
}