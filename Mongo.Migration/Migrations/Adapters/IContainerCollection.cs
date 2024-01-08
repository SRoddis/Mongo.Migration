using System;

namespace Mongo.Migration.Migrations.Adapters;

public interface IContainerCollection
{
    void Register<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface;

    void RegisterInstance<TInterface>(object instance);

    void RegisterSingleton<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface;

    void Register(Type serviceType, Type implementingType);
}