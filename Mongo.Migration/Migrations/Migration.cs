using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;

namespace Mongo.Migration.Migrations
{
    [Obsolete]
    public abstract class Migration<TClass> : DocumentMigration<TClass>
        where TClass : class, IDocument
    {
        protected Migration(string version)
            : base(version)
        {
        }
    }
}