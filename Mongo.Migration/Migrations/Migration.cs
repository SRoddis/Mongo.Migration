using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Document;
using System;

namespace Mongo.Migration.Migrations
{
    [Obsolete]
    public abstract class Migration<TClass> : DocumentMigration<TClass> where TClass : class, IDocument
    {
        protected Migration(string version): base(version)
        {
        }
    }
}
