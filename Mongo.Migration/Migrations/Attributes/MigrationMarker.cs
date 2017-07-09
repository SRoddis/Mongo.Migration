using System;

namespace Mongo.Migration.Migrations.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MigrationMarker : Attribute
    {
    }
}