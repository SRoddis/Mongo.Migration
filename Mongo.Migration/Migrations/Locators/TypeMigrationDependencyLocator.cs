using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationDependencyLocator<TMigrationType> : MigrationLocator<TMigrationType>
        where TMigrationType : class, IMigration
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TypeMigrationDependencyLocator(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                 from type in assembly.GetTypes()
                 where typeof(TMigrationType).IsAssignableFrom(type) && !type.IsAbstract
                 select type).Distinct(new TypeComparer());

            Migrations = migrationTypes.Select(GetMigrationInstance).ToMigrationDictionary();
        }

        private TMigrationType GetMigrationInstance(Type type)
        {
            var constructor = type.GetConstructors()[0];

            using var scopedService = _scopeFactory.CreateScope();
            
            var args = constructor
                .GetParameters()
                .Select(o => o.ParameterType)
                .Select(o => scopedService.ServiceProvider.GetService(o))
                .ToArray();

            return Activator.CreateInstance(type, args) as TMigrationType;
        }

        private class TypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.AssemblyQualifiedName == y.AssemblyQualifiedName;
            }

            public int GetHashCode(Type obj)
            {
                return obj.AssemblyQualifiedName.GetHashCode();
            }
        }
    }
}