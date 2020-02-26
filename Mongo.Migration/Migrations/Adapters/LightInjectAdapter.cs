using System;
using LightInject;

namespace Mongo.Migration.Migrations.Adapters
{
    public class LightInjectAdapter : IContainerAdapter
    {
        private readonly IServiceContainer _container;

        public LightInjectAdapter(IServiceContainer container)
        {
            _container = container;
        }

        public object GetInstance(Type type)
        {
            return _container.GetInstance(type);
        }

        public void Register<TInterface, TImplementation>() where TInterface : class
            where TImplementation : class, TInterface
        {
            _container.Register<TInterface, TImplementation>();
        }
        
        public void RegisterInstance<TInterface>(object instance)
        {
            _container.RegisterInstance(typeof(TInterface), instance);
        }

        public void RegisterSingleton<TInterface, TImplementation>() where TInterface : class
            where TImplementation : class, TInterface
        {
            _container.Register<TInterface, TImplementation>(new PerContainerLifetime());
        }
    }
}