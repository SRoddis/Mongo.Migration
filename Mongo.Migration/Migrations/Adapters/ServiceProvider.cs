using System;

namespace Mongo.Migration.Migrations.Adapters
{
    public class ServiceProvider : IContainerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetInstance(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}