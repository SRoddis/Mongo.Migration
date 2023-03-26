using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators
{
    public class CollectionLocator : AbstractLocator<CollectionLocationInformation, Type>, ICollectionLocator
    {
        public override CollectionLocationInformation? GetLocateOrNull(Type identifier)
        {
            if (!LocatesDictionary.ContainsKey(identifier))
            {
                return null;
            }

            LocatesDictionary.TryGetValue(identifier, out var value);
            return value;
        }

        public override void Locate()
        {
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(CollectionLocation), true)
                where attributes is { Length: > 0 }
                select new { Type = t, Attributes = attributes.Cast<CollectionLocation>() };

            var versions = new Dictionary<Type, CollectionLocationInformation>();

            foreach (var type in types)
            {
                var version = type.Attributes.First().CollectionInformation;
                versions.Add(type.Type, version);
            }

            LocatesDictionary = versions;
        }

        public IDictionary<Type, CollectionLocationInformation> GetLocatesOrEmpty()
        {
            return LocatesDictionary;
        }
    }
}