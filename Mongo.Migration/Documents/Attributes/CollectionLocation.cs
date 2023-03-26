using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionLocation : Attribute
    {
        public CollectionLocationInformation CollectionInformation { get; }

        public CollectionLocation(string collectionName, string databaseName = null)
        {
            CollectionInformation = new CollectionLocationInformation(databaseName, collectionName);
        }
    }
}