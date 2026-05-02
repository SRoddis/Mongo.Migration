using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Documents.Serializers;

public sealed class DocumentVersionSerializer : SerializerBase<DocumentVersion>
{
    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs _,
        DocumentVersion value)
    {
        context.Writer.WriteString(value.ToString());
    }

    public override DocumentVersion Deserialize(BsonDeserializationContext context, BsonDeserializationArgs _)
    {
        var versionString = context.Reader.ReadString();
        return DocumentVersion.Parse(versionString.AsSpan());
    }
}