using System;
using System.Collections.Generic;

using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Migrations.Locators
{
    internal class DatabaseTypeMigrationDependencyLocator : TypeMigrationDependencyLocator<IDatabaseMigration>, IDatabaseTypeMigrationDependencyLocator
    {
        private IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> _migrations;

        protected override IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> Migrations
        {
            get
            {
                if (this._migrations == null)
                {
                    this.Locate();
                }

                return this._migrations;
            }
            set
            {
                this._migrations = value;
            }
        }

        public DatabaseTypeMigrationDependencyLocator(IContainerProvider containerProvider)
            : base(containerProvider)
        {
        }
    }
}