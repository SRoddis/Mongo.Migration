using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    public interface IMigration
    {
        DocumentVersion Version { get; }

        Type Type { get; }
    }
}