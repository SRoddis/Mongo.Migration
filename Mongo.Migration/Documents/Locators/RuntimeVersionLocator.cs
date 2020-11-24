using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators
{
    public class RuntimeVersionLocator : AbstractLocator<DocumentVersion, Type>, IRuntimeVersionLocator
    {
        public override DocumentVersion? GetLocateOrNull(Type identifier)
        {
            if (!LocatesDictionary.ContainsKey(identifier))
                return null;

            LocatesDictionary.TryGetValue(identifier, out var value);
            return value;
        }

        public override void Locate()
        {
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(RuntimeVersion), true)
                where attributes != null && attributes.Length > 0
                select new {Type = t, Attributes = attributes.Cast<RuntimeVersion>()};

            var versions = new Dictionary<Type, DocumentVersion>();

            foreach (var type in types)
            {
                var version = type.Attributes.First().Version;
                versions.Add(type.Type, version);
            }

            LocatesDictionary = versions;
        }
    }
}