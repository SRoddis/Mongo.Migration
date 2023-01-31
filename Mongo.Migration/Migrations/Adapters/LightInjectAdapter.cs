using System;

using LightInject;

namespace Mongo.Migration.Migrations.Adapters
{
    public class LightInjectAdapter : IContainerAdapter
    {
        private readonly IServiceContainer _container;

        public LightInjectAdapter(IServiceContainer container)
        {
            this._container = container;
        }

        public object GetInstance(Type type)
        {
            return this._container.GetInstance(type);
        }

        public void Register<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            this._container.Register<TInterface, TImplementation>();
        }

        public void Register(Type serviceType, Type implementingType)
        {
            this._container.Register(serviceType, implementingType);
        }

        public void RegisterInstance<TInterface>(object instance)
        {
            this._container.RegisterInstance(typeof(TInterface), instance);
        }

        public void RegisterSingleton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            this._container.Register<TInterface, TImplementation>(new PerContainerLifetime());
        }
    }
}