using System;
using LightInject;
using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Test
{
    public class IntegrationTest : IDisposable
    {
        protected const string DatabaseName = "PerformanceTest";
        protected const string CollectionName = "Test";
        protected IMongoClient _client;
        protected IContainerAdapter _containerAdapter;
        protected IComponentRegistry _components;

        protected MongoDbRunner _mongoToGoRunner;

        protected void OnSetUp()
        {
            _mongoToGoRunner = MongoDbRunner.Start();
            _client = new MongoClient(_mongoToGoRunner.ConnectionString);

            _client.GetDatabase(DatabaseName).CreateCollection(CollectionName);

            _containerAdapter = new LightInjectAdapter(new ServiceContainer());

            _components = new ComponentRegistry( new MongoMigrationSettings() {ConnectionString = _mongoToGoRunner.ConnectionString, Database = DatabaseName }, _containerAdapter);
            _components.RegisterComponents(_client);
        }

        public void Dispose()
        {
            _mongoToGoRunner?.Dispose();
        }
    }
}