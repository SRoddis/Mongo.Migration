using LightInject;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.Migration;
using Mongo.Migration.Services.Migration.OnDeserialization;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;
using Mongo.Migration.Services.Migration.OnStartup;
using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    internal class ComponentRegistry : ICompoentRegistry
    {
        private readonly ServiceContainer _container;

        public ComponentRegistry()
        {
            _container = new ServiceContainer();
        }

        public void RegisterMigrationOnStartup(IMongoClient client)
        {
            RegisterDefaults();

            _container.RegisterInstance(client);
            _container.Register<ICollectionMigrationRunner, CollectionMigrationRunner>();
            _container.Register<IMigrationStrategy, MigrationOnStartup>();
        }

        public void RegisterMigrationOnDeserialization()
        {
            RegisterDefaults();
            
            _container.Register<IMigrationRunner, MigrationRunner>();
            _container.Register<IMigrationStrategy, MigrationOnDeserialization>();
            _container.Register<MigrationInterceptorProvider, MigrationInterceptorProvider>();
        }

        public TComponent Get<TComponent>() where TComponent : class
        {
            return _container.GetInstance<TComponent>();
        }

        private void RegisterDefaults()
        {
            _container.Register<IMigrationLocator, TypeMigrationLocator>(new PerContainerLifetime());
            _container.Register<IDatabaseLocator, DatabaseLocator>(new PerContainerLifetime());
            _container.Register<IVersionLocator, VersionLocator>(new PerContainerLifetime());

            _container.Register<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            _container.Register<DocumentVersionSerializer, DocumentVersionSerializer>();

            _container.Register<IMongoMigration, Migration.MongoMigration>();
        }
    }
}