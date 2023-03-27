using System;
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
    internal class MigrationInterceptorFactoryWhenCreating
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
        public void If_type_is_assignable_to_document_Then_interceptor_is_created()
        {
            using var scoped = _serviceProvider.CreateScope();

            var factory = scoped.ServiceProvider.GetRequiredService<IMigrationInterceptorFactory>();

            var interceptor = factory.Create(typeof(TestDocumentWithOneMigration));

            interceptor.ValueType.Should().Be<TestDocumentWithOneMigration>();
        }

        [Test]
        public void If_type_is_not_assignable_to_document_Then_exception_is_thrown()
        {
            using var scoped = _serviceProvider.CreateScope();

            var factory = scoped.ServiceProvider.GetRequiredService<IMigrationInterceptorFactory>();

            Action act = () => factory.Create(typeof(TestClass));

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void If_type_is_null_Then_exception_is_thrown()
        {
            using var scoped = _serviceProvider.CreateScope();

            var factory = scoped.ServiceProvider.GetRequiredService<IMigrationInterceptorFactory>();

            Action act = () => factory.Create(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}