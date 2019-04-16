using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators
{
    public class DatabaseLocator : AbstractLocator<DatabaseLocationInformation, Type>, IDatabaseLocator
    {
        public override DatabaseLocationInformation? GetLocateOrNull(Type identifier)
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
                let attributes = t.GetCustomAttributes(typeof(DatabaseLocation), true)
                where attributes != null && attributes.Length > 0
                select new {Type = t, Attributes = attributes.Cast<DatabaseLocation>()};

            var versions = new Dictionary<Type, DatabaseLocationInformation>();

            foreach (var type in types)
            {
                var version = type.Attributes.First().DatabaseInformation;
                versions.Add(type.Type, version);
            }

            LocatesDictionary = versions;
        }

        public IDictionary<Type, DatabaseLocationInformation> GetLocatesOrEmpty()
        {
            return LocatesDictionary;
        }
    }
}