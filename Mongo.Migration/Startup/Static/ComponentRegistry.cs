using LightInject;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Services.Interceptors;
using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static
{
    internal class ComponentRegistry : IComponentRegistry
    {
        private readonly IMongoMigrationSettings _settings;
        private readonly IContainerAdapter _containerAdapter;

        public ComponentRegistry(IMongoMigrationSettings settings, IContainerAdapter containerAdapter = null)
        {
            _settings = settings;

            if(containerAdapter == null)
                containerAdapter = new LightInjectAdapter(new ServiceContainer());

            _containerAdapter = containerAdapter;
        }

        public void RegisterComponents(IMongoClient client)
        {
            RegisterDefaults();

            _containerAdapter.RegisterInstance<IMongoClient>(client);

            _containerAdapter.Register<IMigrationService, MigrationService>();
        }

        public TComponent Get<TComponent>() where TComponent : class
        {
            return (TComponent) _containerAdapter.GetInstance(typeof(TComponent));
        }

        private void RegisterDefaults()
        {
            _containerAdapter.RegisterInstance<IContainerProvider>(_containerAdapter);
            _containerAdapter.RegisterInstance<IMongoMigrationSettings>(_settings);

            _containerAdapter.RegisterSingleton<IMigrationLocator, TypeMigrationDependencyLocator>();
            _containerAdapter.RegisterSingleton<ICollectionLocator, CollectionLocator>();
            _containerAdapter.RegisterSingleton<IRuntimeVersionLocator, RuntimeVersionLocator>();
            _containerAdapter.RegisterSingleton<IStartUpVersionLocator, StartUpVersionLocator>();

            _containerAdapter.Register<IMigrationLocator, TypeMigrationLocator>();
            _containerAdapter.Register<IAdvancedMigrationLocator, AdvancedTypeMigrationLocator>();
            _containerAdapter.Register<IVersionService, VersionService>();
            _containerAdapter.Register<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
            _containerAdapter.Register<DocumentVersionSerializer, DocumentVersionSerializer>();
            _containerAdapter.Register<ICollectionMigrationRunner, CollectionMigrationRunner>();
            _containerAdapter.Register<IMigrationRunner, MigrationRunner>();
            _containerAdapter.Register<MigrationInterceptorProvider, MigrationInterceptorProvider>();
            _containerAdapter.Register<IDatabaseMigrationRunner, DatabaseMigrationRunner>();
            _containerAdapter.Register<IAdvancedMigrationRunner, AdvancedMigrationRunner>();

            _containerAdapter.Register<IMongoMigration, MongoMigration>();
        }
    }
}