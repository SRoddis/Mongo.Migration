using LightInject;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.Migration;
using Mongo.Migration.Services.Migration.OnDeserialization;
using Mongo.Migration.Services.Migration.OnDeserialization.Interceptors;
using Mongo.Migration.Services.Migration.OnStartup;

namespace Mongo.Migration.Services.Startup.Static
{
    internal class ComponentRegistry : ICompoentRegistry
    {
        private readonly ServiceContainer _container;

        public ComponentRegistry()
        {
            _container = new ServiceContainer();
        }

        public void RegisterMigrationOnStartup()
        {
            _container.Register<IMigrationStrategy, MigrationOnStartup>();

            RegisterDefaults();
        }

        public void RegisterMigrationOnDeserialization()
        {
            _container.Register<DocumentVersionSerializer, DocumentVersionSerializer>();
            _container.Register<MigrationInterceptorProvider, MigrationInterceptorProvider>();
            _container.Register<IMigrationStrategy, MigrationOnDeserialization>();

            RegisterDefaults();
        }

        public TComponent Get<TComponent>() where TComponent : class
        {
            return _container.GetInstance<TComponent>();
        }

        private void RegisterDefaults()
        {
            _container.Register<IMigrationLocator, TypeMigrationLocator>(new PerContainerLifetime());
            _container.Register<IVersionLocator, VersionLocator>(new PerContainerLifetime());

            _container.Register<IMigrationRunner, MigrationRunner>();
            _container.Register<IMigrationInterceptorFactory, MigrationInterceptorFactory>();

            _container.Register<IMongoMigration, Mongo.Migration.MongoMigration>();
        }
    }
}