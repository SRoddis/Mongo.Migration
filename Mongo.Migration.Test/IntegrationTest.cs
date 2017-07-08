using System;
using Mongo.Migration.Services.DiContainer;

namespace Mongo.Migration.Test
{
    internal class IntegrationTest
    {
        protected ComponentRegistry _components;

        public IntegrationTest()
        {
            _components = new ComponentRegistry();
            _components.RegisterComponents();
        }
    }
}