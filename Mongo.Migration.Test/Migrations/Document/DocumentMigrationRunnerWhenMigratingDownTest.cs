using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Startup;
using Mongo.Migration.Test.TestDoubles;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Test.Migrations.Document
{
    [TestFixture]
    internal class DocumentMigrationRunnerWhenMigratingDownTest
    {
        private MongoDbRunner _mongoToGoRunner;
        private ServiceProvider _serviceProvider;

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
        public void When_migrating_down_Then_all_migrations_are_used()
        {
            var document = new BsonDocument
            {
                { "Version", "0.0.2" },
                { "Door", 3 }
            };

            using var scoped = _serviceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDocumentMigrationRunner>(); 
            runner.Run(typeof(TestDocumentWithTwoMigration), document);

            document.Names.ToList()[1].Should().Be("Dors");
            document.Values.ToList()[0].AsString.Should().Be("0.0.0");
        }

        [Test]
        public void When_document_has_Then_all_migrations_are_used_to_that_version()
        {
            var document = new BsonDocument
            {
                { "Version", "0.0.2" },
                { "Door", 3 }
            };

            using var scoped = _serviceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDocumentMigrationRunner>(); 
            runner.Run(typeof(TestDocumentWithTwoMigrationMiddleVersion), document);

            document.Names.ToList()[1].Should().Be("Doors");
            document.Values.ToList()[0].AsString.Should().Be("0.0.1");
        }
    }
}