using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseLocation  : Attribute
    {
        public DatabaseLocation(string databaseName, string collectionName)
        {
            DatabaseInformation = new DatabaseLocationInformation(databaseName, collectionName);
        }

        public DatabaseLocationInformation DatabaseInformation { get;}
    }
}