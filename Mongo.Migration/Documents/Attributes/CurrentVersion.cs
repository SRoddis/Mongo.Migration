using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CurrentVersion : Attribute
    {
        public CurrentVersion(string version)
        {
            Version = version;
        }

        public DocumentVersion Version { get; }
    }
}