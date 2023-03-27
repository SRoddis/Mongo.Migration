using System;
using Mongo.Migration.Resources.Exceptions;

namespace Mongo.Migration.Exceptions
{
    public class MongoMigrationNoMongoClientException : Exception
    {
        public MongoMigrationNoMongoClientException()
            : base(string.Format(ErrorTexts.NoMongoClient))
        {
        }
    }
}