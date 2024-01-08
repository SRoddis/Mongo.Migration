using FluentAssertions;

using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Serializers;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

using NUnit.Framework;

namespace Mongo.Migration.Test.Documents.Serializers;

[TestFixture]
public class DocumentVersionSerializer_when_serialize_and_deserialize
{
    private DocumentVersionSerializer _serializer;

    [Test]
    public void Then_version_is_deserialized_correct()
    {
        // Arrange 
        var document = new BsonDocument { { "version", "0.1.1" } };
        var reader = CreateVersionReader(document);

        var context = BsonDeserializationContext.CreateRoot(reader);
        var args = new BsonDeserializationArgs { NominalType = typeof(DocumentVersion) };

        // Act 
        var result = this._serializer.Deserialize(context, args);

        // Assert 
        result.Should().BeOfType<DocumentVersion>();
        result.Should().Be("0.1.1");
    }

    [Test]
    public void Then_version_is_serialized_correct()
    {
        // Arrange 
        var writer = CreateVersionWriter();
        var context = BsonSerializationContext.CreateRoot(writer);
        var args = new BsonSerializationArgs { NominalType = typeof(DocumentVersion) };
        var version = new DocumentVersion("0.0.1");

        // Act 
        this._serializer.Serialize(context, args, version);

        // Assert 
        var document = writer.Document;
        document.ToString().Should().Be("{ \"version\" : \"0.0.1\" }");
    }

    [SetUp]
    public void SetUp()
    {
        this._serializer = new DocumentVersionSerializer();
    }

    private static BsonDocumentReader CreateVersionReader(BsonDocument document)
    {
        var reader = new BsonDocumentReader(document);
        reader.ReadStartDocument();
        reader.ReadName();
        return reader;
    }

    private static BsonDocumentWriter CreateVersionWriter()
    {
        var writer = new BsonDocumentWriter(new BsonDocument());
        writer.WriteStartDocument();
        writer.WriteName("version");
        return writer;
    }
}