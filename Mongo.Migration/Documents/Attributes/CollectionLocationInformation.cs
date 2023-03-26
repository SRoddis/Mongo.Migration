namespace Mongo.Migration.Documents.Attributes
{
    public struct CollectionLocationInformation
    {
        public CollectionLocationInformation(string database, string collection)
        {
            Database = database;
            Collection = collection;
        }

        public string Database { get; }
        public string Collection { get; }
    }
}