using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionLocation  : Attribute
    {
        public CollectionLocation(string collectionName, string databaseName = null)
        {
            CollectionInformation = new CollectionLocationInformation(databaseName, collectionName);
        }

        public CollectionLocationInformation CollectionInformation { get;}
    }
}