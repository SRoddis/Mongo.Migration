using Mongo.Migration.Services.Startup.Static;

namespace Mongo.Migration.Test
{
    internal class IntegrationTest
    {
        protected ComponentRegistry _components;

        public IntegrationTest()
        {
            _components = new ComponentRegistry();
            _components.RegisterMigrationOnDeserialization();
        }
    }
}