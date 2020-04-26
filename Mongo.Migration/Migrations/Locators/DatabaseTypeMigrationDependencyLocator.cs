using System;
using System.Collections.Generic;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Migrations.Locators
{
    internal class DatabaseTypeMigrationDependencyLocator : TypeMigrationDependencyLocator<IDatabaseMigration>, IDatabaseTypeMigrationDependencyLocator
    {
        private IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> _migrations;

        public DatabaseTypeMigrationDependencyLocator(IContainerProvider containerProvider) : base(containerProvider)
        {
        }

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
            set { _migrations = value; }
        }
    }
}