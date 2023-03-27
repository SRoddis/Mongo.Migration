using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Documents.Attributes;
using Mongo.Migration.Migrations.Locators;

namespace Mongo.Migration.Documents.Locators
{
    internal class StartUpVersionLocator : AbstractLocator<DocumentVersion, Type>, IStartUpVersionLocator
    {
        private readonly IMongoMigrationAssemblyService _mongoMigrationAssemblyService;

        public StartUpVersionLocator(IMongoMigrationAssemblyService mongoMigrationAssemblyService)
        {
            _mongoMigrationAssemblyService = mongoMigrationAssemblyService;
        }
        
        public override DocumentVersion? GetLocateOrNull(Type identifier)
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
                from a in _mongoMigrationAssemblyService.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(StartUpVersion), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<StartUpVersion>() };

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