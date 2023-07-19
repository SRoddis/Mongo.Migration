using System;

using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;

using Mongo2Go;

using MongoDB.Driver;

namespace Mongo.Migration.Test
{
    public class IntegrationTest : IDisposable
    {
        protected IMongoClient _client;

        protected IComponentRegistry _components;

        protected MongoDbRunner _mongoToGoRunner;

        public void Dispose()
        {
            this._mongoToGoRunner?.Dispose();
        }

        protected void OnSetUp()
        {
            this._mongoToGoRunner = MongoDbRunner.Start();
            this._client = new MongoClient(this._mongoToGoRunner.ConnectionString);

            this._client.GetDatabase("PerformanceTest").CreateCollection("Test");

            this._components = new ComponentRegistry(
                new MongoMigrationSettings
                    { ConnectionString = this._mongoToGoRunner.ConnectionString, Database = "PerformanceTest" });
            this._components.RegisterComponents(this._client);
        }
    }
}