using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Migrations.Attributes;

namespace Mongo.Migration.Migrations.Locators
{
    internal class AttributeMigrationLocator : MigrationLocator
    {
        protected override IEnumerable<IMigration> LoadMigrations()
        {
            var typesWithMarker =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(MigrationMaker), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<MigrationMaker>() };

            //TODO: Throw exception if type is not IMigration

            return typesWithMarker.Select(type => (IMigration) Activator.CreateInstance(type.Type));
        }
    }
}