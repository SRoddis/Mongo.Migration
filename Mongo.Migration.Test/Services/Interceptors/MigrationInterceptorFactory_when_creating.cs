using System;
using FluentAssertions;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Test.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Test.Services.Interceptors
{
    [TestFixture]
    internal class MigrationInterceptorFactory_when_creating : IntegrationTest
    {
        [Test]
        public void If_type_is_assignable_to_document_Then_interceptor_is_created()
        {
            // Arrange
            var factory = _components.Get<IMigrationInterceptorFactory>();

            // Act
            var interceptor = factory.Create(typeof(TestDocumentWithOneMigration));

            // Assert
            interceptor.ValueType.Should().Be<TestDocumentWithOneMigration>();
        }
        
        [Test]
        public void If_type_is_not_assignable_to_document_Then_exception_is_thrown()
        {
            // Arrange
            var factory = _components.Get<IMigrationInterceptorFactory>();

            // Act
            Action act = () => factory.Create(typeof(TestClass));

            // Assert
            act.ShouldThrowExactly<ArgumentException>();
        }
        
        [Test]
        public void If_type_is_null_Then_exception_is_thrown()
        {
            // Arrange
            var factory = _components.Get<IMigrationInterceptorFactory>();

            // Act
            Action act = () => factory.Create(null);

            // Assert
            act.ShouldThrowExactly<ArgumentNullException>();
        }  
    }
}