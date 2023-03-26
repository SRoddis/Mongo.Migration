using System;
using Mongo.Migration.Resources.Exceptions;

namespace Mongo.Migration.Exceptions
{
    internal class MongoMigrationDatabaseNotFound
        : Exception
    {
        public MongoMigrationDatabaseNotFound(string databaseName, string valueConnectionString)
            : base(string.Format(ErrorTexts.ConnectionCheckError, databaseName, valueConnectionString))
        {
        }
    }
}