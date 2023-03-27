using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mongo.Migration.Resources.Exceptions;
using Mongo.Migration.Startup;

namespace Mongo.Migration.Migrations.Locators;

public class MongoMigrationAssemblyService : IMongoMigrationAssemblyService
{
    private readonly IMongoMigrationSettings _settings;

    public MongoMigrationAssemblyService(IMongoMigrationSettings settings)
    {
        _settings = settings;
    }
    
    public IEnumerable<Assembly> GetAssemblies()
    {
        var location = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.GetDirectoryName(location);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new DirectoryNotFoundException(ErrorTexts.AppDirNotFound);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            
        return !string.IsNullOrEmpty(_settings.AssemblyPrefix) 
            ? assemblies.Where(x => x.GetName().Name.StartsWith(_settings.AssemblyPrefix)) 
            : assemblies;
    }
}