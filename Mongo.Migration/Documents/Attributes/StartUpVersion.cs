using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StartUpVersion : Attribute
    {
        public StartUpVersion(string version)
        {
            Version = version;
        }

        public DocumentVersion Version { get; }
    }
}