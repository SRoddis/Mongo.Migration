using System;
using Mongo.Migration.Documents;
using MongoDB.Bson;

namespace Mongo.Migration.Migrations
{
    public abstract class Migration<TClass> : Migration<TClass, IDocument> where TClass : class, IDocument
    {
        protected Migration(string version)
            : base(version)
        {
        }
    }
}