using System;

using Mongo.Migration.Documents;

namespace Mongo.Migration.Migrations;

public interface IMigration
{
    DocumentVersion Version { get; }

    Type Type { get; }
}