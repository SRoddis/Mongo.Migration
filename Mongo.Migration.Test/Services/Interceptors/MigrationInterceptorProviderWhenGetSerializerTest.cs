using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Startup;
using Mongo.Migration.Test.TestDoubles;
using Mongo2Go;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services.Interceptors
{
    [TestFixture]
    internal class MigrationInterceptorProviderWhenGetSerializerTest
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
        public void When_entity_is_document_Then_provide_serializer()
        {
            using var scoped = _serviceProvider.CreateScope();

            var provider = scoped.ServiceProvider.GetRequiredService<IMigrationInterceptorProvider>();

            var serializer = provider.GetSerializer(typeof(TestDocumentWithOneMigration));

            serializer.ValueType.Should().Be(typeof(TestDocumentWithOneMigration));
        }

        [Test]
        public void When_entity_is_not_document_Then_provide_null()
        {
            using var scoped = _serviceProvider.CreateScope();

            var provider = scoped.ServiceProvider.GetRequiredService<IMigrationInterceptorProvider>();

            var serializer = provider.GetSerializer(typeof(TestClass));

            serializer.Should().BeNull();
        }
    }
}