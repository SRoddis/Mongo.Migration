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
            using var scopedService = _scopeFactory.CreateScope();

            var mongoAssembly = scopedService.ServiceProvider.GetRequiredService<IMongoMigrationAssemblyService>();

            var migrationTypes = mongoAssembly.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsAssignableTo(typeof(TMigrationType)) && !type.IsAbstract)
                .Distinct(new TypeComparer());

            Migrations = migrationTypes.Select(x => GetMigrationInstance(scopedService.ServiceProvider, x))
                .ToMigrationDictionary();
        }

        private TMigrationType GetMigrationInstance(IServiceProvider serviceProvider, Type type)
        {
            var constructor = type.GetConstructors()[0];


            var args = constructor
                .GetParameters()
                .Select(o => o.ParameterType)
                .Select(serviceProvider.GetService)
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