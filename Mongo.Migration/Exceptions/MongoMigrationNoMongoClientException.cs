using System;

namespace Mongo.Migration.Exceptions;

public class MongoMigrationNoMongoClientException : Exception
{
    public MongoMigrationNoMongoClientException()
        : base(string.Format(ErrorTexts.NoMongoClient))
    {
    }
}