using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;
using Mongo2Go;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Test.MongoDB
{
    [TestFixture]
    internal class MongoRegistratorWhenRegistratingTest
    {
        private ServiceProvider _serviceProvider;
        private MongoDbRunner _mongoToGoRunner;
        
        [SetUp]
        public void SetUp()
        {
            _mongoToGoRunner = MongoDbRunner.Start();
            var client = new MongoClient(_mongoToGoRunner.ConnectionString);
            client.GetDatabase("PerformanceTest").CreateCollection("Test");

            var serviceCollection = new ServiceCollection()
                .AddScoped<IMongoClient>(_ => client)
                .AddMigration(x =>
                {
                    x.ConnectionString = _mongoToGoRunner.ConnectionString;
                    x.Database = "PerformanceTest";
                });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            _mongoToGoRunner.Dispose();
            _serviceProvider.Dispose();
        }

        [Test]
        public void Then_serializer_is_registered()
        {
            using var scoped = _serviceProvider.CreateScope();

            var service = scoped.ServiceProvider.GetRequiredService<IMigrationService>();

            service.Migrate();

            BsonSerializer.LookupSerializer<DocumentVersion>().ValueType.Should().Be(typeof(DocumentVersion));
        }
    }
}