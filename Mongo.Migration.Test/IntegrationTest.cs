using Mongo.Migration.Startup.Static;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Test
{
    public class IntegrationTest
    {
        protected IMongoClient _client;

        protected IComponentRegistry _components;

        protected MongoDbRunner _mongoToGoRunner;

        protected IntegrationTest()
        {
            _mongoToGoRunner = MongoDbRunner.Start();
            _client = new MongoClient(_mongoToGoRunner.ConnectionString);

            _client.GetDatabase("PerformanceTest").CreateCollection("Test");

            _components = new ComponentRegistry();
            _components.RegisterComponents(_client);
        }
    }
}