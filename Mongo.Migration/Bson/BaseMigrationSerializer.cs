using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services;
using Mongo.Migration.Documents;
using MongoDB.Bson.Serialization.Conventions;

namespace Mongo.Migration.Bson;
internal abstract class BaseMigrationSerializer<TDocument> : SerializerBase<TDocument>, IBsonIdProvider, IBsonDocumentSerializer, IBsonPolymorphicSerializer, IHasDiscriminatorConvention
{
    private static readonly Type s_tDocumentType = typeof(TDocument);

    private readonly IDocumentMigrationRunner _migrationRunner;

    protected string VersionFieldName { get; }

    protected DocumentVersion RuntimeVersion { get; }

    protected string RuntimeVersionString { get; }

    protected BsonClassMapSerializer<TDocument> InnerSerializer { get; }

    protected BaseMigrationSerializer(IDocumentMigrationRunner migrationRunner, IDocumentVersionService documentVersionService)
    {
        _migrationRunner = migrationRunner;
        VersionFieldName = documentVersionService.GetVersionFieldName();
        RuntimeVersion = documentVersionService.GetCurrentOrLatestMigrationVersion(s_tDocumentType);
        RuntimeVersionString = RuntimeVersion.ToString();
        BsonClassMap classMap = BsonClassMap.LookupClassMap(s_tDocumentType);
        InnerSerializer = new BsonClassMapSerializer<TDocument>(classMap);
    }

    public override TDocument Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        BsonReaderBookmark bookmark = context.Reader.GetBookmark();

        context.Reader.ReadStartDocument();
        string currentVersion = context.Reader.FindStringElement(VersionFieldName);
        context.Reader.ReturnToBookmark(bookmark);

        if (RuntimeVersionString == currentVersion)
        {
            return InnerSerializer.Deserialize(context, args);
        }

        BsonDocument document = BsonDocumentSerializer.Instance.Deserialize(context);
        _migrationRunner.Run(s_tDocumentType, document, RuntimeVersion);
        var migratedContext = BsonDeserializationContext.CreateRoot(new BsonDocumentReader(document));
        return InnerSerializer.Deserialize(migratedContext, args);
    }

    public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
         => InnerSerializer.TryGetMemberSerializationInfo(memberName, out serializationInfo);

    public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
         => InnerSerializer.GetDocumentId(document, out id, out idNominalType, out idGenerator);

    public void SetDocumentId(object document, object id)
        => InnerSerializer.SetDocumentId(document, id);

    public bool IsDiscriminatorCompatibleWithObjectSerializer
        => InnerSerializer.IsDiscriminatorCompatibleWithObjectSerializer;

    public IDiscriminatorConvention DiscriminatorConvention
         => InnerSerializer.DiscriminatorConvention;
}
