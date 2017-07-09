[![Build status](https://ci.appveyor.com/api/projects/status/t21khtcv66ws18m9?svg=true)](https://ci.appveyor.com/project/SRoddis/mongo-migration) [![Coverage Status](https://coveralls.io/repos/github/SRoddis/Mongo.Migration/badge.svg?branch=master)](https://coveralls.io/github/SRoddis/Mongo.Migration?branch=master)

# Mongo.Migration

Mongo.Migration is designed for [MongoDB C# Driver]( https://github.com/mongodb/mongo-csharp-driver) to migrate your documents easily on-the-fly.
No downtime four simple schema-migrations anymore. Just write small and simple `IMigration`, implement `IDocument` to your entities to allow versioning and deploy it next to your application. Mongo.Migration will handle the rest for you. 

The `MigrationInterceptor<TDocument>` checks on every read/find if the document needs to be migrated and informs the `MigrationRunner` if it is necessary.

`Please NOTE` that updates are not intercepted, because updates don’t provide serialization.

# Installation

Install via nuget https://www.nuget.org/packages/Mongo.Migration

```
PM> Install-Package Mongo.Migration
```

# How to use

1. Initialize `MongoMigration` past the `MongoClient`.
    ```csharp
	// Init MongoMigration
	MongoMigration.Initialize();
    ```
2. Implement `IDocument` or add `Document` to your Entities to provide a `DocumentVersion`. (Optional) add the `CurrentVersion` attribute to mark the current document version. So you have the possibility to migrate down on rollback.

    ```csharp
    [CurrentVersion("0.1.1")]
    public class Car : IDocument
    {
        public ObjectId Id { get; set; }

        public string Type { get; set; }

        public int Doors { get; set; }

        public DocumentVersion Version { get; set; }
    }
    ```
3. Create a `Migration<TDocument>` and mark it with the ``MigrationMarker` attribute. The versioning should be done in [Semantic Versioning]( http://semver.org/). But it is open to you, if you just use the patch version to count the migrations. If there is duplication for a specific type and exception is thrown on initialization.
    ```csharp
    [MigrationMarker]
    public class M001_RenameDorsToDoors : Migration<Car>
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
4. `(Optional)` If you want to place your migrations in an extra project make sure you referance it in the main project and add name it `"*.MongoMigrations"`. Bei convention Mongo.Migration collects all *.dlls with the suffix `".MongoMigrations"` in your bin folder.
    

Compile, run and enjoy! 

## Performance

The performance is measured on every push to the repository with a small performance-test. It measures the time MongoDB need to insert and read `n documents` (5000) without and with Mongo.Migration. The difference is asserted and should be not higher than a given tolerance (150ms).

Example Output of the automated test:
```bash
MongoDB: 73ms, Mongo.Migration: 168ms, Diff: 95ms (Tolerance: 150ms), Documents: 5000, Migrations per Document: 2
```

After bigger changes the code is analyzed with profiling tools to check for performance or memory problems.

## Demo

In the repository you can find [Mongo.Migration.Demo]( https://github.com/SRoddis/Mongo.Migration/tree/master/Mongo.Migration.Demo) which is a simple demo to show how to use Mongo.Migration. 

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

## Next Feature/Todo

	1. Automatically insert after migrating. So migration is done only once. (MongoDB has its performance on read. I will test if it has big performance issues with the automatically insert.)

## Copyright

Copyright © 2017 Sean Roddis

## License

Mongo.Migration is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.
