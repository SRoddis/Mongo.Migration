using DryIoc;
using Mongo.Migration.Models.Serializers;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Services.MongoDB;

namespace Mongo.Migration.Services.DiContainer
{
    internal class ComponentRegistry : ICompoentRegistry
    {
        private readonly Container _container;

        public ComponentRegistry()
        {
            _container = new Container();
        }

        public void RegisterComponents()
        {
            _container.Register<DocumentVersionSerializer, DocumentVersionSerializer>();
            _container.Register<MigrationInterceptorProvider, MigrationInterceptorProvider>();
            _container.Register<IMongoRegistrater, MongoRegistrater>();
            _container.Register<IApplication, Application>();
        }

        public TComponent Get<TComponent>() where TComponent : class
        {
            return _container.Resolve<TComponent>();
        }
    }
}