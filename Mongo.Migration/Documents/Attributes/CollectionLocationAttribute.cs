namespace Mongo.Migration.Documents.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CollectionLocationAttribute : Attribute
{
    public CollectionLocationInformation CollectionInformation { get; }

    public CollectionLocationAttribute(string collectionName)
    {
        CollectionInformation = new CollectionLocationInformation(collectionName);
    }
}