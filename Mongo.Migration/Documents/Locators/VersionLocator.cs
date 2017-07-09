using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators
{
    internal class VersionLocator : IVersionLocator
    {
        private IDictionary<Type, DocumentVersion> _versions;

        private IDictionary<Type, DocumentVersion> Versions
        {
            get
            {
                if (_versions == null)
                    LoadVersions();
                return _versions;
            }
        }

        public DocumentVersion? GetCurrentVersion(Type type)
        {
            if (!Versions.ContainsKey(type))
                return null;

            DocumentVersion value;
            Versions.TryGetValue(type, out value);
            return value;
        }

        public void LoadVersions()
        {
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(CurrentVersion), true)
                where attributes != null && attributes.Length > 0
                select new {Type = t, Attributes = attributes.Cast<CurrentVersion>()};

            var versions = new Dictionary<Type, DocumentVersion>();

            foreach (var type in types)
            {
                var version = type.Attributes.First().Version;
                versions.Add(type.Type, version);
            }

            _versions = versions;
        }
    }
}