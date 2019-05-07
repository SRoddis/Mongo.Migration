using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionVersion : Attribute
    {
        public CollectionVersion(string version)
        {
            Version = version;
        }

        public DocumentVersion Version { get; }
    }
}