using Mongo.Migration.Services.Initializers;
using Mongo2Go;
using MongoDB.Driver;

namespace Mongo.Migration.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Init MongoDB
            MongoDbRunner runner = MongoDbRunner.StartForDebugging();
            var client = new MongoClient(runner.ConnectionString);

            // Init MongoMigration
            MongoMigration.Initialize();
        }
    }
}