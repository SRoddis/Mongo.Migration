using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimeVersion : Attribute
    {
        public DocumentVersion Version { get; }

        public RuntimeVersion(string version)
        {
            Version = version;
        }
    }
}