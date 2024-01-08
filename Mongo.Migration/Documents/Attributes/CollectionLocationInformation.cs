namespace Mongo.Migration.Documents.Attributes;

public struct CollectionLocationInformation
{
    public CollectionLocationInformation(string database, string collection)
    {
        this.Database = database;
        this.Collection = collection;
    }

    public string Database { get; }

    public string Collection { get; }
}