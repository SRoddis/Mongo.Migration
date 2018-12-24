using System;
using LightInject;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Services.MongoDB;

namespace Mongo.Migration.Services.DiContainer
{
    internal class ComponentRegistry : IComponentRegistry
    {
        private readonly ServiceContainer _container;

        public ComponentRegistry()
        {
            _container = new ServiceContainer();
        }

        public void RegisterComponents<TBaseDocument>(Func<TBaseDocument, DocumentVersion> versionGetter, Action<TBaseDocument, DocumentVersion> versionSetter)
        {
            _container.Register<DocumentVersionSerializer, DocumentVersionSerializer>();
            _container.Register<MigrationInterceptorProvider<TBaseDocument>, MigrationInterceptorProvider<TBaseDocument>>();
            _container.Register<IMigrationLocator, TypeMigrationLocator>(new PerContainerLifetime());
            _container.Register<IVersionLocator, VersionLocator>(new PerContainerLifetime());

            _container.RegisterInstance<Func<TBaseDocument, DocumentVersion>>(versionGetter);
            _container.RegisterInstance<Action<TBaseDocument, DocumentVersion>>(versionSetter);
            _container.Register<IMigrationRunner<TBaseDocument>, MigrationRunner<TBaseDocument>>();

            //_container.Register<Func<TBaseDocument, DocumentVersion>, Action<TBaseDocument, DocumentVersion>, MigrationRunner<TBaseDocument>>((factory, vGetter, vSetter) => 
            //    new MigrationRunner<TBaseDocument>(factory.GetInstance<IMigrationLocator>(), factory.GetInstance<IVersionLocator>(), versionGetter, versionSetter));


            _container.Register<IMigrationInterceptorFactory, MigrationInterceptorFactory<TBaseDocument>>();
            _container.Register<IMongoRegistrator, MongoRegistrator<TBaseDocument>>();
            _container.Register<IApplication, Application>();
        }

        public TComponent Get<TComponent>() where TComponent : class
        {
            return _container.GetInstance<TComponent>();
        }
    }
}