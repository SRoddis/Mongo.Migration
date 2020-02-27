﻿using LightInject;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Services.Interceptors;
using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    internal class ComponentRegistry : IComponentRegistry
    {
        private readonly ServiceContainer _container;

        public ComponentRegistry()
        {
            _container = new ServiceContainer();
        }

        public void RegisterComponents(IMongoClient client)
        {
            RegisterDefaults();

            _container.RegisterInstance(client);
            
            _container.Register<IMigrationService, MigrationService>();
        }

        public TComponent Get<TComponent>() where TComponent : class
        {
            return _container.GetInstance<TComponent>();
        }

        private void RegisterDefaults()
        {
            _container.Register<IMigrationLocator, TypeMigrationLocator>(new PerContainerLifetime());
            _container.Register<IAdvancedMigrationLocator, AdvancedTypeMigrationLocator>(new PerContainerLifetime());
            _container.Register<ICollectionLocator, CollectionLocator>(new PerContainerLifetime());
            _container.Register<IRuntimeVersionLocator, RuntimeVersionLocator>(new PerContainerLifetime());
            _container.Register<IStartUpVersionLocator, StartUpVersionLocator>(new PerContainerLifetime());

            _container.Register<IVersionService, VersionService>();
            _container.Register<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            _container.Register<DocumentVersionSerializer, DocumentVersionSerializer>();

            _container.Register<ICollectionMigrationRunner, CollectionMigrationRunner>();
            _container.Register<IDatabaseMigrationRunner, DatabaseMigrationRunner>();
            _container.Register<IMigrationRunner, MigrationRunner>();
            _container.Register<IAdvancedMigrationRunner, AdvancedMigrationRunner>();
            _container.Register<MigrationInterceptorProvider, MigrationInterceptorProvider>();

            _container.Register<IMongoMigration, MongoMigration>();
        }
    }
}