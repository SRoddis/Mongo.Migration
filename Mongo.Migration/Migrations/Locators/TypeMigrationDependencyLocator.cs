using System;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Adapters;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationDependencyLocator<TMigrationType> : MigrationLocator<TMigrationType>
        where TMigrationType: class, IMigration
    {
        private readonly IContainerProvider _containerProvider;

        public TypeMigrationDependencyLocator(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }
        
        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                from type in assembly.GetTypes()
                where typeof(TMigrationType).IsAssignableFrom(type) && !type.IsAbstract
                select type).Distinct();

            Migrations = migrationTypes.Select(GetMigrationInstance).ToMigrationDictionary();
        }

        private TMigrationType GetMigrationInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];

            if(constructor != null)
            {
                object[] args = constructor
                    .GetParameters()
                    .Select(o => o.ParameterType)
                    .Select(o =>  _containerProvider.GetInstance(o))
                    .ToArray();

                return Activator.CreateInstance(type, args) as TMigrationType;
            }

            return  Activator.CreateInstance(type) as TMigrationType;
        }
    }
}