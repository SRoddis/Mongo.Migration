using System;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Utils
{
    public class MigrationUtils
    {
        private readonly ILogger logger;

        public MigrationUtils()
        {
            this.logger = ApplicationLogging.CreateLogger();
        }

        public long UpdateInnerDocuments(
            IMongoDatabase db,
            BsonDocument documentToUpdate,
            string collection,
            string documentField) => this.UpdateInnerDocuments(db, documentToUpdate, collection, documentField, "_id");

        public long UpdateInnerDocuments(
            IMongoDatabase db,
            BsonDocument documentToUpdate,
            string collection,
            string documentField,
            string fieldToFilter)
        {
            logger.LogInformation($"Atualizando inner documents: {collection}.{documentField} ({fieldToFilter})...");

            var fieldToFilterValue = documentToUpdate.GetValue(fieldToFilter);

            var filter = Builders<BsonDocument>.Filter.Eq($"{documentField}.{fieldToFilter}", fieldToFilterValue);
            var update = Builders<BsonDocument>.Update.Set(documentField, documentToUpdate);
            var result = db.GetCollection<BsonDocument>(collection).UpdateMany(filter, update);

            logger.LogInformation($"Foram modificados {result.ModifiedCount} registros " +
                $"da collection {collection}, {documentField}.{fieldToFilter}={fieldToFilterValue}.");

            return result.ModifiedCount;
        }

        public static BsonArray GetTranslatableFieldArray(string nameEn, string namePt)
        {
            return new BsonArray {
                new BsonDocument { { "lang", "en" }, { "value", nameEn } },
                new BsonDocument { { "lang", "pt" }, { "value", namePt } }
            };
        }

        public static BsonValue TryGetValue(BsonDocument document, string attr, ILogger logger)
        {
            try
            {
                return document[attr];
            }
            catch (Exception)
            {
                logger.LogWarning($"O atributo {attr} não foi encontrado no documento, retornando o valor null");
                return BsonNull.Value;
            }
        }

        public static void UpdateUniqueIndex(IMongoDatabase db, string collectionName, string field, string indexName, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Atualizando indice {collectionName}.{field}");

                var collection = db.GetCollection<BsonDocument>(collectionName);
                collection.Indexes.DropOne(indexName);

                var collation = new Collation("en", strength: new Optional<CollationStrength?>(CollationStrength.Secondary));

                var indexModel = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending(field), new CreateIndexOptions
                {
                    Unique = true,
                    Collation = collation,
                    Name = indexName
                });

                collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao criar o indice de unique para o campo {collectionName}.{field}");
            }
        }
        public static void CreateAscendingIndex(IMongoDatabase db, string collectionName, string field, string indexName, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Criando indice ascendente: {collectionName}.{field}");

                var collection = db.GetCollection<BsonDocument>(collectionName);

                var indexModel = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending(field), new CreateIndexOptions
                {
                    Name = indexName
                });

                collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao criar o indice ascending para o campo {collectionName}.{field}");
            }
        }

        public static void CreateDescendingIndex(IMongoDatabase db, string collectionName, string field, string indexName, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Criando indice descending: {collectionName}.{field}");

                var collection = db.GetCollection<BsonDocument>(collectionName);

                var indexModel = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Descending(field), new CreateIndexOptions
                {
                    Name = indexName
                });

                collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao criar o indice descending para o campo {collectionName}.{field}");
            }
        }

        public static void CreateTextIndex(IMongoDatabase db, string collectionName, string field, string indexName, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Criando indice texto: {collectionName}.{field}");

                var collection = db.GetCollection<BsonDocument>(collectionName);

                var indexModel = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Text(field), new CreateIndexOptions
                {
                    Name = indexName
                });

                collection.Indexes.CreateOne(indexModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao criar o indice texto para o campo {collectionName}.{field}");
            }
        }
    }
}