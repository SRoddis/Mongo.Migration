using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services;
using MongoDB.Bson.Serialization;
using System.Linq.Expressions;

namespace Mongo.Migration.Bson;
internal sealed class MigrationReflexionSerializer<TDocument> : BaseMigrationSerializer<TDocument>
{
    private readonly Action<TDocument, string> _versionSetter;

    public MigrationReflexionSerializer(IDocumentMigrationRunner migrationRunner,
        IDocumentVersionService documentVersionService)
        : base(migrationRunner, documentVersionService)
    {
        _versionSetter = CreateSetter<TDocument, string>(VersionFieldName);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TDocument value)
    {
        _versionSetter.Invoke(value, RuntimeVersion);
        InnerSerializer.Serialize(context, args, value);
    }

    private static Action<T, TProperty> CreateSetter<T, TProperty>(string propertyName)
    {
        var objParam = Expression.Parameter(typeof(T), "x");
        var valueParam = Expression.Parameter(typeof(TProperty), "value");
        var property = Expression.Property(objParam, propertyName);
        var assign = Expression.Assign(property, valueParam);
        var lambda = Expression.Lambda<Action<T, TProperty>>(assign, objParam, valueParam);
        return lambda.Compile();
    }
}
