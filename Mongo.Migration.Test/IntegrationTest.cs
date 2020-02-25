using System;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Test
{
    public class IntegrationTest : IDisposable
    {
        protected IMongoClient _client;

        protected IComponentRegistry _components;

        protected MongoDbRunner _mongoToGoRunner;

        protected void OnSetUp()
        {
            _mongoToGoRunner = MongoDbRunner.Start();
            _client = new MongoClient(_mongoToGoRunner.ConnectionString);

            _client.GetDatabase("PerformanceTest").CreateCollection("Test");

            _components = new ComponentRegistry( new MongoMigrationSettings() {ConnectionString = _mongoToGoRunner.ConnectionString, Database = "PerformanceTest"});
            _components.RegisterComponents(_client);
        }

        public void Dispose()
        {
            _mongoToGoRunner?.Dispose();
        }
    }
}