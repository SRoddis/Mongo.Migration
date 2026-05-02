namespace Mongo.Migration.Documents.Attributes;

public readonly struct CollectionLocationInformation
{
    public CollectionLocationInformation(string collection)
    {
        Collection = collection;
    }

    public string Collection { get; }
}