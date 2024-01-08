using LightInject;

using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using Mongo.Migration.Services.Interceptors;

using MongoDB.Driver;

namespace Mongo.Migration.Startup.Static;

internal class ComponentRegistry : IComponentRegistry
{
    private readonly IContainerAdapter _containerAdapter;

    private readonly IMongoMigrationSettings _settings;

    public ComponentRegistry(IMongoMigrationSettings settings, IContainerAdapter containerAdapter = null)
    {
        this._settings = settings;

        if (containerAdapter == null)
        {
            containerAdapter = new LightInjectAdapter(new ServiceContainer());
        }

        this._containerAdapter = containerAdapter;
    }

    public void RegisterComponents(IMongoClient client)
    {
        this.RegisterDefaults();

        this._containerAdapter.RegisterInstance<IMongoClient>(client);

        this._containerAdapter.Register<IMigrationService, MigrationService>();
    }

    public TComponent Get<TComponent>()
        where TComponent : class
    {
        return (TComponent)this._containerAdapter.GetInstance(typeof(TComponent));
    }

    private void RegisterDefaults()
    {
        this._containerAdapter.RegisterInstance<IContainerProvider>(this._containerAdapter);

        this._containerAdapter.Register(typeof(IMigrationLocator<>), typeof(TypeMigrationDependencyLocator<>));

        this._containerAdapter.RegisterInstance<IMongoMigrationSettings>(this._settings);

        this._containerAdapter.RegisterSingleton<ICollectionLocator, CollectionLocator>();
        this._containerAdapter.RegisterSingleton<IDatabaseTypeMigrationDependencyLocator, DatabaseTypeMigrationDependencyLocator>();
        this._containerAdapter.RegisterSingleton<IRuntimeVersionLocator, RuntimeVersionLocator>();
        this._containerAdapter.RegisterSingleton<IStartUpVersionLocator, StartUpVersionLocator>();

        this._containerAdapter.Register<IDocumentVersionService, DocumentVersionService>();
        this._containerAdapter.Register<IDatabaseVersionService, DatabaseVersionService>();
        this._containerAdapter.Register<IMigrationInterceptorFactory, MigrationInterceptorFactory>();
        this._containerAdapter.Register<DocumentVersionSerializer, DocumentVersionSerializer>();

        this._containerAdapter.Register<IStartUpDocumentMigrationRunner, StartUpDocumentMigrationRunner>();
        this._containerAdapter.Register<IDocumentMigrationRunner, DocumentMigrationRunner>();
        this._containerAdapter.Register<IMigrationInterceptorProvider, MigrationInterceptorProvider>();

        this._containerAdapter.Register<IStartUpDatabaseMigrationRunner, StartUpDatabaseMigrationRunner>();
        this._containerAdapter.Register<IDatabaseMigrationRunner, DatabaseMigrationRunner>();

        this._containerAdapter.Register<IMongoMigration, MongoMigration>();
    }
}