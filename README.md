[![GitHub License](https://img.shields.io/github/license/sroddis/Mongo.Migration)](https://github.com/sroddis/Mongo.Migration/tree/master?tab=MIT-1-ov-file)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mongo.Migration)](https://www.nuget.org/packages/Mongo.Migration/)
[![NuGet](https://img.shields.io/nuget/v/Mongo.Migration)](https://www.nuget.org/packages/Mongo.Migration/)
[![GitHub last commit](https://img.shields.io/github/last-commit/sroddis/Mongo.Migration)](https://github.com/sroddis/Mongo.Migration/commits/master/)

# Mongo.Migration

![](https://media.giphy.com/media/10tLOFXDFDjgQM/giphy.gif)

Mongo.Migration is designed for the [MongoDB C# Driver](https://github.com/mongodb/mongo-csharp-driver) to migrate your documents easily and on-the-fly.
No more downtime for schema-migrations. Just write small and simple `migrations`.

The library is still based on the official [MongoDB.Driver](https://www.mongodb.com/docs/drivers/csharp/) (3.5.2+) and supports the following 3 types of migration:

- **Runtime document migration:**    
    This kind of migration is executed at serialization/deserialization process. It allows your application to consume data serialized at another version
- **Startup document migration:**  
    This kind of migration can be executed on demand (generally at startup) and execute the same document migrations using bulk writes.  
    This migration is slow and memory consuming, but it could be enough for small data sets
- **Database migration:**
    This kind of migration give access to the entire database and allows to write much faster migrations. They also allow to manage indexes, rights, or any other maintenance of the database.

**Notes:**  A robust application will probably use two kind of migrations.

- The database migration executed by the continuous integration 
- The runtime document migration executed by the runtime to prevent any error during the database migration

# Installation

Install via nuget [Mongo.Migration](https://www.nuget.org/packages/Mongo.Migration):  

```shell
dotnet add package Mongo.Migration
```

# Register migration services

```c#
string myConnectionString = "...";
IServiceCollection services = new ServiceCollection();
services
    .AddLogging(builder => builder.AddProvider(NullLoggerProvider.Instance))    // Require logging
    .AddSingleton<IMongoClient>(new MongoClient(myConnectionString)             // Require IMongoClient
    .AddMigration(builder =>                                                    // Configure the migration (null => all migration types enabled)
    {
        builder
            .AddDocumentMigratedType<MyPOCO>("0.0.1")                           // Declare POCO outside assembly for runtime migration (requires an existing string Version property)
            .AddRuntimeDocumentMigration()
            .AddStartupDocumentMigration()
            .AddDatabaseMigration()
    });
```

# Execute migrations

```c#
IServiceProvider provider = services.BuildServiceProvider();

IMigrationService migrationService = provider
    .GetRequiredService<IMigrationService>();

// Register serializers in MongoDb.Driver and enable runtime migration if setup
// Must be called once before any mongo call
migrationService.RegisterBsonStatics();

// Execute the migrations
await migrationService
    .ExecuteDatabaseMigrationAsync("my-database", "1.0.0");

await migrationService
    .ExecuteDocumentMigrationAsync("my-database");
```

# Document migrations quick Start 
Document migrations first usage is to be executed at runtime on POCO serialize/deserialize.  
You can still execute them at startup to migrate your database but this **is not efficient** and should only be done for **very small volumes of data**.

1. Implement `IDocument` or add `Document` to your entities to provide the `DocumentVersion`. (Optional) Add the `RuntimeVersion` attribute to mark the current version of the document. So you have the possibility to downgrade in case of a rollback.
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
2. Create a migration by extending the abstract class `DocumentMigration<TDocument>`. Best practice for the version is to use [Semantic Versioning,](http://semver.org/) but ultimately it is up to you. You could simply use the patch version to count the number of migrations. If there is a duplicate for a specific type an exception is thrown on initialization.
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
3. `(Optional)` If you choose to put your migrations into an extra project, 
add the suffix `".MongoMigrations"` to the name and make sure it is referenced in the main project. By convention Mongo.Migration collects all .dlls named like that in your bin folder.
    
Compile, run and enjoy!

# Database migrations quick start

Database migrations are very efficient to migrate large volume of data, and it allows to manage indexes or whatever you need.  
Here are some tips to implement your migrations:
- Do not create transactions
- Try to write atomic updates as most as possible
- Create many small migrations
- Also migrate your documents and update their runtime version
- Write the most robust migrations possible (specially atomicity isn't guaranteed)
- Do not depend on your data model (it will change, you migrations shouldn't)

1. Create a migration by extending the abstract class `DatabaseMigration`. Best practice for the version is to use [Semantic Versioning,](http://semver.org/) but ultimately it is up to you. You could simply use the patch version to count the number of migrations. All database migrations you add for a database will be executed at StartUp.
    ```csharp
        public class M110AddNewWheels : DatabaseMigration
        {
        private const string PreviousCarVersion = "3.5.0";
        private const string NewCarVersion = "3.6.0";
        
            public M110AddNewWheels() : base("1.1.0") { }
        
            public override async Task UpAsync(IMongoDatabase db, CancellationToken cancellationToken)
            {
                var collection = db.GetCollection<BsonDocument>("Car");
                await collection.UpdateManyAsync(
                    Builders<BsonDocument>.Filter.Eq("Version", PreviousCarVersion), // update only previous documents
                    Builders<BsonDocument>.Update
                        .Set("Wheels", 4)
                        .Set(nameof(IDocument.Version), NewCarVersion), // updates in atomic operation
                    null,
                    cancellationToken
                );
            }
        
            public override async Task DownAsync(IMongoDatabase db, CancellationToken cancellationToken)
            {
                var collection = db.GetCollection<BsonDocument>("Car");
                await collection.UpdateManyAsync(
                    Builders<BsonDocument>.Filter.Eq("Version", NewCarVersion), // update only new documents
                    Builders<BsonDocument>.Update
                        .Unset("Wheels")
                        .Set(nameof(IDocument.Version), PreviousCarVersion),
                    null,
                    cancellationToken
                );
            }
        }
    ```

# Attributes

## RuntimeVersion

Add `RuntimeVersion` attribute to mark the current version of the document. So you have the possibility to downgrade in case of a rollback.
If you do not set the `RuntimeVersion`, all migrations will be applied.

```csharp
[RuntimeVersion("0.0.1")]   
public class Car : IDocument { }
```

## CollectionLocation

Add `CollectionLocation` attribute if you want to migrate your collections at startup with document migration.  
This attribute tells Mongo.Migration where to find your Collections.

```csharp
[CollectionLocation("Car")]
public class Car : IDocument { }
```
## StartUpVersion
Add `StartUpVersion` attribute to set the version you want to migrate to at startup with document migration. 
This attribute limits the migrations to be performed on startup.

```csharp
[StartUpVersion("0.0.1")]
public class Car : IDocument { }
```

# Suggestions

Deploy the migrations in a separate artifact. Otherwise, you lose the ability to downgrade in case of a rollback.

# Release notes

## v5.1.0
This version mostly introduce .Net 10 support.

### Updates
- .Net version update (.net10.0, .net9.0, .net8.0)
- MongoDB.Driver@3.5.2 (required for .Net 10)

## v5.0.0
This could be not 100% exhaustive but v5.0.0 did a lot of changes comparing to older versions.
Consider also there was a lot of changes between the last 3.1.4 officially published version and the source code.

### Updates
- .Net version update (.net8_0, .net9_0)
- MongDB.Driver@3.0.0+
- Dependency updates
- Remove Mongo2Go in favor of Testcontainers
- Refactoring initialisation
  - Can migrate multiple database
  - Remove CollectionLocationAttribute `Database` property
  - Can enable separately all migration types
  - Add extension method to initialize before app startup
- Use span for DocumentVersion parsing
- Use mongo bookmark when no migration needed
- A lot of cleanup and optimization
- Documentation rewriting
- More tests

### Breaking changes
- Remove .Net framework support
- MongoDB.Driver@3.0.0
- DatabaseMigration now use async methods for UpAsync and DownAsync
- CollectionLocationAttribute Database property removed
- Refactoring initialisation

# Next Feature/Todo

1. Create startup setting to limit database migrations ran at startup on fresh database
2. Add real benchmark for runtime migrations

# License
Mongo.Migration is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to [LICENSE.md](LICENSE.md) for more information.
