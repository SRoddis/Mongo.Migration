using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mongo.Migration.Demo.Model.Pkg;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Migration.Demo.WebCore.Pgk
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private IMongoClient _client;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var connectionString = _configuration.GetSection("MongoDb:ConnectionString").Value;
            var databaseName = _configuration.GetSection("MongoDb:Database").Value;
            
            //var runner = MongoDbRunner.Start();
            _client = new MongoClient(connectionString);
            services.AddSingleton<IMongoClient>(_client);
            
            CreateTestDocuments();

            services.Configure<MongoMigrationSettings>(
                options =>
                {
                    options.ConnectionString = connectionString; //With Mongo2Go use: runner.ConnectionString;
                    options.Database = databaseName;
                });
            services.AddMigration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.Run(
                async context =>
                {
                    try
                    {
                        var connectionString = _configuration.GetSection("MongoDb:ConnectionString").Value;
            
                        //var runner = MongoDbRunner.Start();
                        var client = new MongoClient(connectionString);
                        
                        // Migrate old version to current version by reading collection
                        var typedCollection = client.GetDatabase("Local-test").GetCollection<Car>("Car");
                        
                        // Create new car and add it with current version number into MongoDB
                        var id = ObjectId.GenerateNewId();
                        var type = "Test" + id;
                        var car = new Car {Doors = 2, Type = type};

                        typedCollection.InsertOne(car);
                        var test = typedCollection.FindAsync(Builders<Car>.Filter.Eq(c => c.Type, type)).Result.Single();

                        var aggregate = typedCollection.Aggregate()
                            .Match(new BsonDocument {{"Dors", 3}});
                        var results = aggregate.ToListAsync().Result;

                        var result = typedCollection.FindAsync(_ => true).Result.ToListAsync().Result;

                        var response = "";
                        result.ForEach(
                            d =>
                            {
                                response += d.ToBsonDocument().ToString() + "\n";            
                            });
                    
                        await context.Response.WriteAsync(response);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                });
        }
        
        private void CreateTestDocuments()
        {
            _client.GetDatabase("Local-test").DropCollection("Car");

            // Insert old and new version of cars into MongoDB
            var cars = new List<BsonDocument>
            {
                new BsonDocument {{"Dors", 3}, {"Type", "Cabrio"}, {"UnnecessaryField", ""}},
                new BsonDocument {{"Dors", 5}, {"Type", "Combi"}, {"UnnecessaryField", ""}},
                new BsonDocument {{"Doors", 3}, {"Type", "Truck"}, {"UnnecessaryField", ""}, {"Version", "0.0.1"}},
                new BsonDocument {{"Doors", 5}, {"Type", "Van"}, {"Version", "0.1.1"}}
            };

            var bsonCollection =
                _client.GetDatabase("Local-test").GetCollection<BsonDocument>("Car");

            bsonCollection.InsertManyAsync(cars).Wait();
        }
    }
}