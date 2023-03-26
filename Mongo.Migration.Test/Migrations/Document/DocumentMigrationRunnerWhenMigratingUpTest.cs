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
    internal class DocumentMigrationRunnerWhenMigratingUpTest
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
        public void When_migrate_up_the_lowest_version_Then_all_migrations_are_used()
        {
            var document = new BsonDocument
            {
                { "Version", "0.0.0" },
                { "Dors", 3 }
            };

            using var scoped = _serviceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDocumentMigrationRunner>(); 
            runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_no_version_Then_all_migrations_are_used()
        {
            var document = new BsonDocument
            {
                { "Dors", 3 }
            };

            using var scoped = _serviceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDocumentMigrationRunner>(); 
            runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_current_version_Then_nothing_happens()
        {
            var document = new BsonDocument
            {
                { "Version", "0.0.2" },
                { "Door", 3 }
            };

            using var scoped = _serviceProvider.CreateScope();
            var runner = scoped.ServiceProvider.GetRequiredService<IDocumentMigrationRunner>(); 
            runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }
    }
}