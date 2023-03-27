using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup;
using Mongo.Migration.Test.TestDoubles;
using Mongo2Go;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services
{
    [TestFixture]
    internal class DocumentVersionServiceWhenDetermineVersionTest
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
        public void When_document_has_current_version_Then_current_version_is_set()
        {
            var document = new TestDocumentWithTwoMigrationMiddleVersion();
            
            using var scoped = _serviceProvider.CreateScope();
            var service = scoped.ServiceProvider.GetRequiredService<IDocumentVersionService>(); 
            service.DetermineVersion(document);

            document.Version.Should().Be("0.0.1");
        }

        [Test]
        public void When_document_has_highest_version_Then_highest_version_is_set()
        {
            var document = new TestDocumentWithTwoMigrationHighestVersion();
            
            using var scoped = _serviceProvider.CreateScope();
            var service = scoped.ServiceProvider.GetRequiredService<IDocumentVersionService>(); 
            service.DetermineVersion(document);


            document.Version.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_version_that_should_not_be_Then_throw_exception()
        {
            var document = new TestDocumentWithTwoMigrationHighestVersion { Version = "0.0.1" };
            
            using var scoped = _serviceProvider.CreateScope();
            var service = scoped.ServiceProvider.GetRequiredService<IDocumentVersionService>(); 

            Action checkAction = () => { service.DetermineVersion(document); };

            checkAction.Should().Throw<VersionViolationException>();
        }
    }
}