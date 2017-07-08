using LightInject;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Models.Serializers;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Services.MongoDB;

namespace Mongo.Migration.Services.DiContainer
{
    internal class ComponentRegistry : ICompoentRegistry
    {
        private readonly ServiceContainer _container;

        public ComponentRegistry()
        {
            _container = new ServiceContainer();
        }

        public void RegisterComponents()
        {
            _container.Register<DocumentVersionSerializer, DocumentVersionSerializer>();
            _container.Register<MigrationInterceptorProvider, MigrationInterceptorProvider>();
            _container.Register<IMigrationLocator, AttributeMigrationLocator>();

            _container.Register<IMigrationRunner, MigrationRunner>();
            _container.Register<IMongoRegistrater, MongoRegistrater>();
            _container.Register<IApplication, Application>();
        }

        public TComponent Get<TComponent>() where TComponent : class
        {
            return _container.GetInstance<TComponent>();
        }
    }
}