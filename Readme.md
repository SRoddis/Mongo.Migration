[![.NET](https://github.com/SRoddis/Mongo.Migration/actions/workflows/dotnet.yml/badge.svg)](https://github.com/SRoddis/Mongo.Migration/actions/workflows/dotnet.yml)
[![Coverage Status](https://coveralls.io/repos/github/SRoddis/Mongo.Migration/badge.svg)](https://coveralls.io/github/SRoddis/Mongo.Migration)
[![NuGet](https://img.shields.io/nuget/dt/Mongo.Migration.svg)](https://www.nuget.org/packages/Mongo.Migration/)
[![NuGet](https://img.shields.io/nuget/v/Mongo.Migration.svg)](https://www.nuget.org/packages/Mongo.Migration/)
![GitHub last commit](https://img.shields.io/github/last-commit/sroddis/Mongo.Migration.svg)

# Mongo.Migration


![](https://media.giphy.com/media/10tLOFXDFDjgQM/giphy.gif)


Mongo.Migration is designed for the [MongoDB C# Driver](https://github.com/mongodb/mongo-csharp-driver) to migrate your documents easily and on-the-fly.
No more downtime for schema-migrations. Just write small and simple `migrations`.

`**Edit**`

With version 3.0.0 of Mongo.Migration I added the possibility to run migrations on StartUp. In order to keep the core of Mongo.Migration in focus, it is still possible to run migrations at runtime (on-the-fly). In addition, there is now the option of executing migrations at the start of the application.

`**PLEASE NOTE**` If you use on-the-fly migration updates, aggregation pipeline and projections are not handled, because they don’t use serialization. You have to handle them yourself.

With version 4.0.0 of Mongo.Migration was added the possibility to run database migrations on StartUp. 
Now exist additional option of executing migrations which can manipulate the mongo database insert documents, rename collections and other operations that you need. It's more generic operations than only manipulate document when exists.

# Installation

Install via nuget https://www.nuget.org/packages/Mongo.Migration

```
PM> Install-Package Mongo.Migration
```

# Document migrations quick Start 

#### .Net Core

1.1 Add `MongoMigration` with the StartupFilter (`IMongoClient` has to be registered at the DI-container before)

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    _client = new MongoClient( _configuration.GetSection("MongoDb:ConnectionString").Value);
    
    services.AddSingleton<IMongoClient>(_client);
                
    services.AddMigration();
}
```
1.2 Add `MongoMigration` with the StartupFilter add connection setting to use separate client
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    _client = new MongoClient( _configuration.GetSection("MongoDb:ConnectionString").Value);
    
    services.AddSingleton<IMongoClient>(_client);
                
    services.AddMigration(new MongoMigrationSettings
    {
        ConnectionString = _configuration.GetSection("MongoDb:ConnectionString").Value,
        Database = _configuration.GetSection("MongoDb:Database").Value, 
        VersionFieldName = "TestVersionName" // Optional
    });
}
```

    
2. Implement `IDocument` or add `Document` to your entities to provide the `DocumentVersion`. (Optional) Add the `RuntimeVersion` attribute to mark the current version of the document. So you have the possibility to downgrade in case of a rollback.

```csharp
[RuntimeVersion("0.0.1")]
public class Car : IDocument
{
    public ObjectId Id { get; set; }

    public string Type { get; set; }

    public int Doors { get; set; }

    public DocumentVersion Version { get; set; }
}
```
3. Create a migration by extending the abstract class `DocumentMigration<TDocument>`. Best practice for the version is to use [Semantic Versioning](http://semver.org/) but ultimately it is up to you. You could simply use the patch version to count the number of migrations. If there is a duplicate for a specific type an exception is thrown on initialization.

```csharp
public class M001_RenameDorsToDoors : DocumentMigration<Car>
{
    public M001_RenameDorsToDoors()
        : base("0.0.1")
    {
    }

    public override void Up(BsonDocument document)
    {
        var doors = document["Dors"].ToInt32();
        document.Add("Doors", doors);
        document.Remove("Dors");
    }

    public override void Down(BsonDocument document)
    {
        var doors = document["Doors"].ToInt32();
        document.Add("Dors", doors);
        document.Remove("Doors");
    }
}
```
4. `(Optional)` If you choose to put your migrations into an extra project, 
add the suffix `".MongoMigrations"` to the name and make sure it is referenced in the main project. By convention Mongo.Migration collects all .dlls named like that in your bin folder.
    
Compile, run and enjoy!

## How to use document migration

With version 3.0.0 of Mongo.Migration I added the possibility to run migrations on StartUp. 
In order to keep the core of Mongo.Migration in focus, it is still possible to run migrations at runtime (on-the-fly). 
In addition, there is now the option of executing migrations at the start of the application.

#### Document migration at runtime

See `Quick Start ` 

#### Document migration on startup

If you want to run migrations on StartUp, the only thing you have to do is add the attribute `CollectionLocation`. 
Now all migrations you add for a `IDocument` will be executed at StartUp.

```csharp
    [CollectionLocation("Car", "TestCars")]
    public class Car : IDocument
    {
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public int Doors { get; set; }

        public DocumentVersion Version { get; set; }
    }
```

Additionally you can fix the version of the document with  `StartUpVersion`

```csharp
    [StartUpVersion("0.1.1")]
    [CollectionLocation("Car", "TestCars")]
    public class Car : IDocument
    {
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public int Doors { get; set; }

        public DocumentVersion Version { get; set; }
    }
```

`**PLEASE NOTE**` 
Mongo.Migration uses the IStartUpFilter for .net core. 
Maybe you want to read this [article](https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/), to check if there is a better option to migrate with Mongo.Migration at StartUp.

#### Document migration on startup and at runtime

This is an example how you can use both.
At startup the version will be 0.0.1 and at runtime, when a document will be deserialized the version will be migrated to 0.1.1

```csharp
    [RuntimeVersion("0.1.1")]
    [StartUpVersion("0.0.1")]
    [CollectionLocation("Car", "TestCars")]
    public class Car : IDocument
    {
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public int Doors { get; set; }

        public DocumentVersion Version { get; set; }
    }
```


### Annotations

#### RuntimeVersion
Add `RuntimeVersion` attribute to mark the current version of the document. So you have the possibility to downgrade in case of a rollback.
If you do not set the `RuntimeVersion`, all migrations will be applied.

```csharp
[RuntimeVersion("0.0.1")]   
public class Car : IDocument
...
```
#### CollectionLocation
Add `CollectionLocation` attribute if you want to migrate your collections at startup. This attribute tells Mongo.Migration where to find your Collections.

```csharp
[CollectionLocation("Car", "TestCars")]
public class Car : IDocument
...   
```
#### StartUpVersion
Add `StartUpVersion` attribute to set the version you want to migrate to at startup. This attribute limits the migrations to be performed on startup

```csharp
[StartUpVersion("0.0.1")]
public class Car : IDocument
...
```

# Database migrations quick start

#### .Net Core

1.1 Add `MongoMigration` with the StartupFilter (`IMongoClient` has to be registered at the DI-container before)

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();

    _client = new MongoClient( _configuration.GetSection("MongoDb:ConnectionString").Value);
    
    services.AddSingleton<IMongoClient>(_client);
                
    services.AddMigration(new MongoMigrationSettings()
                         {
                             ConnectionString = runner.ConnectionString,
                             Database = "TestCars"
                         });
}

```
1.2 Add `MongoMigration` with the StartupFilter add connection setting to use separate client
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
                
    services.AddMigration(new MongoMigrationSettings
    {
        ConnectionString = _configuration.GetSection("MongoDb:ConnectionString").Value,
        Database = _configuration.GetSection("MongoDb:Database").Value, 
        DatabaseMigrationVersion = new DocumentVersion(2,0,0) // Optional
    });
}
```

    
2. Create a migration by extending the abstract class `DatabaseMigration`. Best practice for the version is to use [Semantic Versioning](http://semver.org/) but ultimately it is up to you. You could simply use the patch version to count the number of migrations. All database migrations you add for a database will be executed at StartUp.

```csharp
    public class M100_AddNewCar : DatabaseMigration
    {
        public M100_AddNewCar()
            : base("1.0.0")
        {
        }

        public override void Up(IMongoDatabase db)
        {
            var collection = db.GetCollection<Car>("Car");
            collection.InsertOne(new Car
            {
                Doors = 123,
                Type = "AddedInMigration"
            });
        }

        public override void Down(IMongoDatabase db)
        {
            var collection = db.GetCollection<Car>("Car");
            collection.DeleteOne(Builders<Car>.Filter.Eq(c => c.Type, "AddedInMigration"));
        }
    }
```

### Annotations
#### CollectionLocation
`CollectionLocation` attribute if you not specify database in MongoMigrationSettings then database from attribure will be use at startup.

#### Setup current database version
Database will downgrade to current version using database migrations
```csharp
new MongoMigrationSettings()
{
    ...
    DatabaseMigrationVersion = new DocumentVersion(2,0,0)
}
``` 

## Dependency injection
With the latest update (3.0.94) I added a requested feature to `Mongo.Migration`. `Migration` can be injected with dependencies from now on.

#### .NetCore
It is pitty simple with .NetCore. `Mongo.Migration` uses the `IServiceProvider` to resolve all used dependencies. So you have access to all registered dependencies.


```csharp
    public class M001_RenameDorsToDoors : DocumentMigration<Car>
    {
        private readonly IYourDependency _service;

        public M001_RenameDorsToDoors(IYourDependency service)
            : base("0.0.1")
        {
            _service = service;
        }

    ...
```

## Document migrations demo

Inside of the repository you can find a [Mongo.Migration.Demo]( https://github.com/SRoddis/Mongo.Migration/tree/master/Mongo.Migration.Demo) which is a simple demo to show how to use Mongo.Migration. 

1. Compile and run the demo application.
2. Now you should see the following output in the console.

```bash
	Migrate from:
	{ "_id" : ObjectId("59624d5beb5bb330386cd859"), "Dors" : 3, "Type" : "Cabrio", "UnnecessaryField" : "" }

	{ "_id" : ObjectId("59624d5beb5bb330386cd85a"), "Dors" : 5, "Type" : "Combi", "UnnecessaryField" : "" }

	{ "_id" : ObjectId("59624d5beb5bb330386cd85b"), "Doors" : 3, "Type" : "Truck", "UnnecessaryField" : "", "Version" : "0.0.1" }

	{ "_id" : ObjectId("59624d5beb5bb330386cd85c"), "Doors" : 5, "Type" : "Van", "Version" : "0.1.1" }

	To:
	{ "_id" : ObjectId("59624d5beb5bb330386cd859"), "Type" : "Cabrio", "Doors" : 3, "Version" : "0.1.1" }

	{ "_id" : ObjectId("59624d5beb5bb330386cd85a"), "Type" : "Combi", "Doors" : 5, "Version" : "0.1.1" }

	{ "_id" : ObjectId("59624d5beb5bb330386cd85b"), "Type" : "Truck", "Doors" : 3, "Version" : "0.1.1" }

	{ "_id" : ObjectId("59624d5beb5bb330386cd85c"), "Type" : "Van", "Doors" : 5, "Version" : "0.1.1" }

	New Car was created with version: 0.1.1

	Press any Key to exit...
```

3. `(Optional)` Run [Mongo.Migration.Demo.Performance.Console]( https://github.com/SRoddis/Mongo.Migration/tree/master/Mongo.Migration.Demo.Performance.Console)


## Database migrations demo

Inside of the repository you can find a [Mongo.Migration.Database.Demo]( https://github.com/SRoddis/Mongo.Migration/tree/master/Mongo.Migration.Database.Demo) which is a simple demo to show how to use database Mongo.Migration. 

1. Compile and run the demo application.
2. Now you should see the following output in the console.

```bash
Apply database migrations:


{ "_id" : ObjectId("5ead80b68f9c4c402c35ebb9"), "MigrationId" : "Mongo.Migration.Database.Demo.Migrations.M100_AddNewCar", "Version" : "1.0.0" }

{ "_id" : ObjectId("5ead80b68f9c4c402c35ebba"), "MigrationId" : "Mongo.Migration.Database.Demo.Migrations.M200_UpdateNewCar", "Version" : "2.0.0" }

{ "_id" : ObjectId("5ead80b78f9c4c402c35ebbb"), "MigrationId" : "Mongo.Migration.Database.Demo.Migrations.M300_AddSparePartsCollection", "Version" : "3.0.0" }

New Car was added and updated in database migrations:
{ "_id" : ObjectId("5ead80b68f9c4c402c35ebb8"), "Type" : "AddedInMigration", "Doors" : 222 }

New collection was added in database:
SparePart



Press any Key to exit...
```


## Suggestions

Deploy the migrations in a separate artifact. Otherwise you lose the ability to downgrade in case of a rollback.

## Performance

The performance is measured on every push to the repository with a small performance-test. It measures the time MongoDB needs to insert and read `n documents` (5000) with and without Mongo.Migration. The difference is asserted and should be not higher than a given tolerance (150ms).

Example output of the automated test:
```bash
MongoDB: 73ms, Mongo.Migration: 168ms, Diff: 95ms (Tolerance: 150ms), Documents: 5000, Migrations per Document: 2

MongoDB: 88ms, Mongo.Migration: 109ms, Diff: 21ms (Tolerance: 70ms), Documents: 1500, Migrations per Document: 2

MongoDB: 62ms, Mongo.Migration: 63ms, Diff: 1ms (Tolerance: 40ms), Documents: 100, Migrations per Document: 2

MongoDB: 48ms, Mongo.Migration: 50ms, Diff: 2ms (Tolerance: 10ms), Documents: 10, Migrations per Document: 2

```

After bigger changes the code is analyzed with profiling tools to check for performance or memory problems.

## Copyright

Copyright © 2018 Sean Roddis

## License

Mongo.Migration is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.
