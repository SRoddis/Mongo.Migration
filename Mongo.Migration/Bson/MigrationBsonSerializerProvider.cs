using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Bson;

internal sealed class MigrationBsonSerializerProvider : IRegistryAwareBsonSerializationProvider
{
    private static readonly Type s_migrationDocumentSerializerGenericType = typeof(MigrationDocumentSerializer<>);
    private static readonly Type s_iDocumentType = typeof(IDocument);
    private static readonly Type s_migrationReflexionSerializerGenericType = typeof(MigrationReflexionSerializer<>);

    private readonly IRuntimeVersionLocator _runtimeVersionLocator;
    private readonly object[] _constructorParameters;

    public MigrationBsonSerializerProvider(IDocumentMigrationRunner migrationRunner,
        IDocumentVersionService documentVersionService,
        IRuntimeVersionLocator runtimeVersionLocator)
    {
        _runtimeVersionLocator = runtimeVersionLocator;
        _constructorParameters = [migrationRunner, documentVersionService];
    }

    public IBsonSerializer GetSerializer(Type type)
    {
        return GetSerializer(type, BsonSerializer.SerializerRegistry);
    }

    public IBsonSerializer GetSerializer(Type type, IBsonSerializerRegistry serializerRegistry)
    {
        if (type.GetInterfaces().Contains(s_iDocumentType))
        {
            var genericType = s_migrationDocumentSerializerGenericType.MakeGenericType(type);
            return (IBsonSerializer)Activator.CreateInstance(genericType, _constructorParameters)!;
        }

        if (_runtimeVersionLocator.GetLocateOrNull(type) is not null)
        {
            var genericType = s_migrationReflexionSerializerGenericType.MakeGenericType(type);
            return (IBsonSerializer)Activator.CreateInstance(genericType, _constructorParameters)!;
        }

        return null!;
    }
}