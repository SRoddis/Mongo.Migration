using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Mongo.Migration.Documents;
using Mongo.Migration.Test.TestDoubles;
using Mongo.Migration.Migrations.Locators;

namespace Mongo.Migration.Test.Migrations
{
    public class CollectionMigrationRunner_when_migrating_a_typed_document : IntegrationTest
    {
        private ICollectionMigrationRunner _runner;
        private IMongoCollection<BsonDocument> _collection;

        [SetUp]
        public async Task SetUp()
        {
         
            base.OnSetUp();
            
            //mock ICollectionLocator for migration Type to execute
            var collectionLocatorMock = new Mock<ICollectionLocator>();
            collectionLocatorMock.Setup(x => x.GetLocatesOrEmpty())
                       .Returns(new Dictionary<Type, CollectionLocationInformation>() 
                       { 
                           {typeof(Car), new CollectionLocationInformation(DatabaseName,CollectionName)} 
                       });

            var migrationLocatorMock = new Mock<IMigrationLocator>();
            migrationLocatorMock.Setup(x => x.GetMigrationsFromTo(It.Is<Type>(t => t==typeof(Car)),
                                                                  It.IsAny<DocumentVersion>(),It.IsAny<DocumentVersion>()))
                                .Returns(new[] {new CarMigration_0_0_1() });

            migrationLocatorMock.Setup(x => x.GetLatestVersion(It.Is<Type>(t => t == typeof(Car))))
                                .Returns(new DocumentVersion(0, 0, 1));
            
                _containerAdapter.RegisterInstance<ICollectionLocator>(collectionLocatorMock.Object);
            _containerAdapter.RegisterInstance<IMigrationLocator>(migrationLocatorMock.Object);

            _collection = _client.GetDatabase(DatabaseName).GetCollection<BsonDocument>(CollectionName);
            await SeedDatabaseAsync();

            _runner = _components.Get<ICollectionMigrationRunner>();

        }

        private async Task SeedDatabaseAsync()
        {
            var vehicleData = new[]
            { 
                GetCar(),
                GetBoat()
            };

            await _collection.InsertManyAsync(vehicleData);
        }


        [TearDown]
        public void TearDown()
        {
            this.Dispose();
        }

        [Test] public async Task when_migrating_a_typed_document_then_only_that_type_is_returned_in_the_batch()
        {
            _runner.RunAll();
            var results = (await _collection.FindAsync(FilterDefinition<BsonDocument>.Empty)).ToList();
            var migratedResult = results.Where(x => x.Contains("Version")).SingleOrDefault();

            results.Count().Should().Be(2);
            migratedResult.Should().NotBeNull();
            migratedResult.Should().BeEquivalentTo(GetCarWithVersion());
        }

        #region testData
        private static BsonDocument GetCar()
        {
            return new BsonDocument { { "_id", new ObjectId("5e9ff3856c9cab88f0a1cf1f") }, { "_t", new BsonArray() { "Vehicle", "Car" } }, { "Doors", 3 }, { "Type", "Cabrio" }, };
        }

        private static BsonDocument GetCarWithVersion()
        {
            var car = GetCar();
            car.Add(new BsonElement("Version", "0.0.1"));
            return car;
        }

        private static BsonDocument GetBoat()
        {
            return new BsonDocument { { "_id", new ObjectId("5e9ff3856c9cab88f0a1cf2f") }, { "_t", new BsonArray() { "Vehicle", "Boat" } }, { "Propellers", 3 }, { "Type", "Bertram 35" }, };
        }


        #endregion

    }
}
