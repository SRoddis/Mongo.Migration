using System;

namespace Mongo.Migration.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CurrentDocumentVersion : Attribute
    {
        public CurrentDocumentVersion(string version)
        {
            Version = version;
        }

        public DocumentVersion Version { get; }
    }
}