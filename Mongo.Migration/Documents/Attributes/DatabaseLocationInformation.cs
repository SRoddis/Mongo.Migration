namespace Mongo.Migration.Documents.Attributes
{
    public struct DatabaseLocationInformation
    {
        public DatabaseLocationInformation(string database, string collection)
        {
            Database = database;
            Collection = collection;
        }        
        
        public string Database { get; }
        public string Collection { get; }
    }
}