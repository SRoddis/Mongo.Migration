using System;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Adapters;

namespace Mongo.Migration.Migrations.Locators
{
    public class TypeMigrationDependencyLocator : MigrationLocator
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
                where typeof(IMigration).IsAssignableFrom(type) && !type.IsAbstract
                select type).Distinct();

            Migrations = migrationTypes.Select(GetMigrationInstance).ToMigrationDictionary();
        }

        private IMigration GetMigrationInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];

            if(constructor != null)
            {
                object[] args = constructor
                    .GetParameters()
                    .Select(o => o.ParameterType)
                    .Select(o =>  _containerProvider.GetInstance(o))
                    .ToArray();

                return Activator.CreateInstance(type, args) as IMigration;
            }

            return  Activator.CreateInstance(type) as IMigration;
        }
    }
}